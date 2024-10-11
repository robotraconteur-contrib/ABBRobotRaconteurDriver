using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Abb.Egm;
using Google.Protobuf;

namespace ABBRobotRaconteurDriver
{
    public class EgmClient : IDisposable
    {

        int port;
        bool keep_going = false;

        UdpClient egm_socket;
        Thread thread;
        Func<long> now_func;

        public void Start(int egm_port, Func<long> now_func, int joint_count)
        {            
            port = egm_port;            
            keep_going = true;
            this.now_func = now_func;

            egm_sensor.Header = new EgmHeader();
            egm_sensor.Header.Seqno = 0;
            egm_sensor.Header.Tm = 0;
            egm_sensor.Header.Mtype = EgmHeader.Types.MessageType.MsgtypeCorrection;
            egm_sensor.Planned = new EgmPlanned();
            egm_sensor.Planned.Joints = new EgmJoints();
            egm_sensor.Planned.Joints.Joints.AddRange(new double[joint_count]);

            thread = new Thread(_run);
            thread.Start();
        }

        EgmRobot egm_robot = new EgmRobot();
        EgmSensor egm_sensor = new EgmSensor();

        public Exception LastException { get; private set; }

        void _run()
        {
            while (keep_going)
            {
                try
                {
                    using (egm_socket = new UdpClient(6510))
                    {
                        while (keep_going)
                        {
                            IPEndPoint ep = null;
                            DoRecvEgm(egm_socket, ref ep);
                            DoSendEgm(egm_socket, ep);
                        }
                        return;
                    }
                }
                catch (Exception e)
                {
                    if (!keep_going)
                        return;

                    Console.WriteLine($"Robot communication error: {e.ToString()}");
                    LastException = e;
                }

                for (int i = 0; i < 10; i++)
                {
                    if (!keep_going)
                        break;
                    Thread.Sleep(100);
                }

                Console.WriteLine($"Retrying robot connection!");

            }

        }

        double[] joint_cmd;

        private void DoSendEgm(UdpClient egm_socket, IPEndPoint ep)
        {
            if (joint_cmd != null)
            {
                for (int i = 0; i < joint_cmd.Length; i++)
                {
                    egm_sensor.Planned.Joints.Joints[i] = joint_cmd[i] * (180.0/Math.PI);
                }

                var egm_sensor_bytes = egm_sensor.ToByteArray();
                egm_socket.Send(egm_sensor_bytes, egm_sensor_bytes.Length, ep);
            }
        }

        long last_recv;
        double[] actual_joint_position;
        com.robotraconteur.geometry.Pose tcp_pose = default;

        bool ready;
        bool enabled;

        private void DoRecvEgm(UdpClient egm_socket, ref IPEndPoint ep)
        {            
            var recv = egm_socket.Receive(ref ep);
            egm_robot = new EgmRobot();
            egm_robot.MergeFrom(new CodedInputStream(recv));
            lock (this)
            {
                if (egm_robot.HasFeedBack && egm_robot.FeedBack.HasJoints && egm_robot.FeedBack.HasCartesian && egm_robot.HasMotorState && egm_robot.HasRapidExecState)
                {
                    last_recv = now_func();
                    var joint_msg = egm_robot.FeedBack.Joints.Joints;
                    if (actual_joint_position == null || actual_joint_position.Length != joint_msg.Count)
                    {
                        actual_joint_position = new double[joint_msg.Count];
                    }

                    for (int i=0; i<joint_msg.Count; i++)
                    {
                        actual_joint_position[i] = joint_msg[i] * (Math.PI/180.0);
                    }

                    var cart = egm_robot.FeedBack.Cartesian;
                    tcp_pose.position.x = cart.Pos.X/1000.0;
                    tcp_pose.position.y = cart.Pos.Y/1000.0;
                    tcp_pose.position.z = cart.Pos.Z/1000.0;
                    tcp_pose.orientation.w = cart.Orient.U0;
                    tcp_pose.orientation.x = cart.Orient.U1;
                    tcp_pose.orientation.y = cart.Orient.U2;
                    tcp_pose.orientation.z = cart.Orient.U3;

                    enabled = egm_robot.HasMotorState;
                    ready = egm_robot.RapidExecState.State == EgmRapidCtrlExecState.Types.RapidCtrlExecStateType.RapidRunning;
                }
            }
        }

        public void Dispose()
        {
            keep_going = false;
            egm_socket?.Close();
        }

        public void GetState(out long last_recv, out bool enabled, out bool ready, out double[] joint_pos, out com.robotraconteur.geometry.Pose tcp_pose)
        {
            lock(this)
            {
                last_recv = this.last_recv;
                joint_pos = this.actual_joint_position;
                tcp_pose = this.tcp_pose;
                enabled = this.enabled;
                ready = this.ready;
            }
        }

        bool motion_stopped = false;

        public void StopMotion()
        {
            lock(this)
            {
                if (!motion_stopped)
                {
                    joint_cmd = actual_joint_position;
                    motion_stopped = true;
                }
            }
        }

        public void SetCommandPosition(double[] position)
        {
            lock (this)
            {
                motion_stopped = false;
                if (actual_joint_position != null)
                {
                    if (position.Length != actual_joint_position.Length)
                        throw new ArgumentException("Invalid joint array length");
                }            
                joint_cmd = position;
            }
        }
    }
}
