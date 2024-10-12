from RobotRaconteur.Client import *
import time
import numpy as np

c = RRN.ConnectService('rr+tcp://localhost:58651?service=robot')

robot_info = c.robot_info
print(robot_info)

print(c.robot_state.PeekInValue()[0].command_mode)

robot_const = RRN.GetConstants("com.robotraconteur.robotics.robot", c)

joint_names = [j.joint_identifier.name for j in robot_info.joint_info]

halt_mode = robot_const["RobotCommandMode"]["halt"]
trajectory_mode = robot_const["RobotCommandMode"]["trajectory"]

JointTrajectoryWaypoint = RRN.GetStructureType("com.robotraconteur.robotics.trajectory.JointTrajectoryWaypoint", c)
JointTrajectory = RRN.GetStructureType("com.robotraconteur.robotics.trajectory.JointTrajectory", c)

c.command_mode = halt_mode
time.sleep(0.1)
c.command_mode = trajectory_mode

state_w = c.robot_state.Connect()

state_w.WaitInValueValid()
state1 = state_w.InValue

waypoints = []

j_start = state1.joint_position
j_end = [0, 0, 0, 0, 0, 0]

for i in range(11):
    wp = JointTrajectoryWaypoint()
    wp.joint_position = (j_end - j_start) * (float(i) / 10.0) + j_start
    wp.time_from_start = i / 2.0
    waypoints.append(wp)

traj = JointTrajectory()
traj.joint_names = joint_names
traj.waypoints = waypoints

c.speed_ratio = 1

traj_gen = c.execute_trajectory(traj)

while (True):
    t = time.time()

    robot_state = state_w.InValue

    res, gen_state = traj_gen.TryNext()
    if not res:
        break

    print(gen_state)
    print(hex(c.robot_state.PeekInValue()[0].robot_state_flags))

waypoints = []

for i in range(101):
    t = float(i) / 10.0
    wp = JointTrajectoryWaypoint()
    cmd = np.deg2rad(15) * np.sin(2 * np.pi * (t / 10.0)) * np.array([1, 0, 0, 0, 0.5, -1])
    cmd = cmd + j_end
    wp.joint_position = cmd
    wp.time_from_start = t
    waypoints.append(wp)

traj = JointTrajectory()
traj.joint_names = joint_names
traj.waypoints = waypoints

c.speed_ratio = 1.0

traj_gen = c.execute_trajectory(traj)

while (True):
    t = time.time()

    robot_state = state_w.InValue

    res, gen_state = traj_gen.TryNext()
    if not res:
        break

    print(gen_state)
