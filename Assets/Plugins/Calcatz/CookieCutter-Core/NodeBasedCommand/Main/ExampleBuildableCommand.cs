using UnityEngine;

namespace Calcatz.CookieCutter {
    public class ExampleBuildableCommand : BuildableCommand {

        public override float nodeWidth => 200f;

        public ExampleBuildableCommand() {
            AddPropertyInput<bool>("A bool", true);
            AddPropertyInput<int>("An int", 4);
            AddPropertyInput<string>("A string", "blabla");
            AddPropertyInput<float>("A float", 54f);
            AddPropertyInput<Vector3>("A vector3", new Vector3(3, 4, 5));
            AddPropertyInput<GameObject>("A ref object", null);

            AddPropertyOutput<int>("Out int", 99);
            AddPropertyOutput<float>("Out float", 3f);
            AddPropertyOutput<string>("Out string", "asdasd");
            AddPropertyOutput<Vector3>("Out vector3", Vector3.right);
            AddPropertyOutput<bool>("Out bool", false);
            AddPropertyOutput<bool>("Out bool 2", false);
            AddPropertyOutput<AudioClip>("Out ref object", null);

            AddMainOutput("Main alt 1");
            AddMainOutput("Main alt 2");
            AddMainOutput("Main alt 3");
        }

        public override void Execute(CommandExecutionFlow _flow) {
            //Getting the value of "A string", which is at in-point index 2
            Debug.Log(GetInput<string>(_flow, 2));
            base.Execute(_flow);
        }

    }
}
