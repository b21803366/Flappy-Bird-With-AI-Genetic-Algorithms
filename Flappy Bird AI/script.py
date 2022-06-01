import mlagents
from mlagents_envs.environment import UnityEnvironment



env = UnityEnvironment(file_name='.\\train\mlBirdV2', seed=1, side_channels=[])
env.reset()
behavior_name = list(env.behavior_specs)[0]
print(f"Name of the behavior : {behavior_name}")
spec = env.behavior_specs[behavior_name]


print("Number of observations : ", len(spec.observation_shapes))

if spec.is_action_continuous():
  print("The action is continuous")

if spec.is_action_discrete():
  print("The action is discrete")

decision_steps, terminal_steps = env.get_steps(behavior_name)
print(decision_steps.obs)
env.close()