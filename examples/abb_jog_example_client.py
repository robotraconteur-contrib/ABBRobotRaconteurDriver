from RobotRaconteur.Client import *
import time
import numpy as np

c = RRN.ConnectService('rr+tcp://localhost:58651?service=robot')

robot_info = c.robot_info
print(robot_info)

print(c.robot_state.PeekInValue()[0].command_mode)

robot_const = RRN.GetConstants("com.robotraconteur.robotics.robot", c)

halt_mode = robot_const["RobotCommandMode"]["halt"]
jog_mode = robot_const["RobotCommandMode"]["jog"]

c.command_mode = halt_mode
time.sleep(0.1)
c.command_mode = jog_mode

joint_diff = np.array([-5, 0, 0, 0, 0, 5, 0]) * np.deg2rad(1)
joint_vel = np.array([5.0, 1, 1, 1, 1, 1])

for i in range(5):
    t = time.time()
    print("Begin move")
    c.jog_freespace(np.deg2rad([15, 0, 0, 0, 0, 0]), joint_vel, True)
    print("done")
    time.sleep(0.5)
    print(hex(c.robot_state.PeekInValue()[0].robot_state_flags))
    print("Begin move")
    c.jog_freespace(np.deg2rad([-15, 0, 0, 0, 0, 0]), joint_vel, True)
    print("Done")
    print(hex(c.robot_state.PeekInValue()[0].robot_state_flags))
    time.sleep(0.5)
