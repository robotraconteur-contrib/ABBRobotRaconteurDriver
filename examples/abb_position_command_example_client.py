from RobotRaconteur.Client import *
import time
import numpy as np

c = RRN.ConnectService('rr+tcp://localhost:58651?service=robot')
# c = RRN.ConnectService('rr+local:///?nodename=sawyer_robot&service=sawyer')

robot_info = c.robot_info
print(robot_info)

# c.reset_errors()

# c.enable()

# time.sleep(10)

print(c.robot_state.PeekInValue()[0].command_mode)

robot_const = RRN.GetConstants("com.robotraconteur.robotics.robot", c)

halt_mode = robot_const["RobotCommandMode"]["halt"]
position_mode = robot_const["RobotCommandMode"]["position_command"]

RobotJointCommand = RRN.GetStructureType("com.robotraconteur.robotics.robot.RobotJointCommand", c)

# c.reset_errors()

c.command_mode = halt_mode
time.sleep(0.1)
c.command_mode = position_mode

cmd_w = c.position_command.Connect()
state_w = c.robot_state.Connect()

state_w.WaitInValueValid()

init_joint_pos = state_w.InValue.joint_position

command_seqno = 1

t_start = time.time()

while (True):
    t = time.time()

    robot_state = state_w.InValue

    command_seqno += 1
    joint_cmd1 = RobotJointCommand()
    joint_cmd1.seqno = command_seqno
    joint_cmd1.state_seqno = robot_state.seqno
    cmd = np.deg2rad([0, 0, 0, 0, 15, 15]) * np.sin(t - t_start) + init_joint_pos
    joint_cmd1.command = cmd

    cmd_w.OutValue = joint_cmd1

    state_flags = state_w.InValue.robot_state_flags
    if (not (state_flags & (robot_const["RobotStateFlags"]["ready"]))):
        print("Robot not ready")
        break

    time.sleep(.01)

    if (t - t_start > 10):
        break
