// Copyright 2020 Rensselaer Polytechnic Institute
//                Wason Technology, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using RobotRaconteur;
using com.robotraconteur.robotics.robot;
using System.IO;
using System.Linq;
using com.robotraconteur.robotics.joints;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using com.robotraconteur.geometry;
using com.robotraconteur.action;
using com.robotraconteur.robotics.trajectory;
using RobotRaconteur.Companion.Robot;

namespace ABBRobotRaconteurDriver
{
    public class ABBRobot : AbstractRobot
    {

        EgmClient egm_client;

        public ABBRobot(com.robotraconteur.robotics.robot.RobotInfo robot_info) : base(robot_info, -1)
        {
            _uses_homing = false;
            _has_position_command = true;
            _has_velocity_command = false;
            _update_period = 4;
            robot_info.robot_capabilities &= (uint)(RobotCapabilities.jog_command & RobotCapabilities.position_command & RobotCapabilities.trajectory_command);
            
        }

        public override void _start_robot()
        {
            egm_client = new EgmClient();
            egm_client.Start(6510, () => _now, _joint_count);
            base._start_robot();
        }

        protected override Task _send_disable()
        {
            throw new NotImplementedException();
        }

        protected override Task _send_enable()
        {
            throw new NotImplementedException();
        }

        protected override Task _send_reset_errors()
        {
            throw new NotImplementedException();
        }

        protected override void _send_robot_command(long now, double[] joint_pos_cmd, double[] joint_vel_cmd)
        {
            if (joint_pos_cmd != null)
            {
                egm_client.SetCommandPosition(joint_pos_cmd);
            }
        }

        protected override void _run_timestep(long now)
        {
            egm_client.GetState(out var egm_last_recv, out bool egm_enabled, out bool egm_ready, out var egm_joint_pos, out var egm_tcp_pos);
            // TODO: Need to get operational mode!
            _operational_mode = RobotOperationalMode.auto;
            _last_joint_state = egm_last_recv;
            _last_endpoint_state = egm_last_recv;
            _last_robot_state = egm_last_recv;
            _enabled = egm_enabled;
            _ready = egm_ready;
            if (egm_joint_pos != null)
            {
                _joint_position = egm_joint_pos;
            }
            else
            {
                _joint_position = new double[0];
            }
            _endpoint_pose = new Pose[] { egm_tcp_pos };

            base._run_timestep(now);

            if (_command_mode == RobotCommandMode.halt || _command_mode == RobotCommandMode.invalid_state)
            {
                egm_client.StopMotion();
            }
        }

        public override void Dispose()
        {
            egm_client?.Dispose();
            base.Dispose();
        }
    }
}
