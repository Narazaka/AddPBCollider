using nadena.dev.ndmf;
using nadena.dev.ndmf.localization;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

[assembly: ExportsPlugin(typeof(Narazaka.VRChat.AddPBCollider.Editor.AddPBColliderPlugin))]

namespace Narazaka.VRChat.AddPBCollider.Editor
{
    public class AddPBColliderPlugin : Plugin<AddPBColliderPlugin>
    {
        protected override void Configure()
        {
            InPhase(BuildPhase.Transforming).AfterPlugin("wataameya.marshmallow_PB.ndmf").Run("AddPBColliderPlugin", Pass);
        }

        void Pass(BuildContext ctx)
        {
            var addPBColliders = ctx.AvatarRootObject.GetComponentsInChildren<AddPBCollider>(true);
            foreach (var addPBCollider in addPBColliders)
            {
                AddPbCollider(addPBCollider);
                Object.DestroyImmediate(addPBCollider);
            }
        }

        void AddPbCollider(AddPBCollider addPBCollider)
        {
            var pb = addPBCollider.targetPb;
            if (pb == null)
            {
                using (ErrorReport.WithContextObject(addPBCollider))
                {
                    ErrorReport.ReportError(new PhysboneNotFoundError(addPBCollider));
                }
                return;
            }
            var physBones = pb.GetComponents<VRCPhysBone>();
            if (physBones.Length <= addPBCollider.pbIndex)
            {
                using (ErrorReport.WithContextObject(addPBCollider))
                {
                    ErrorReport.ReportError(new PhysboneNotFoundError(addPBCollider));
                }
                return;
            }
            var physBone = physBones[addPBCollider.pbIndex];
            physBone.colliders.AddRange(addPBCollider.colliders);
        }

        static readonly Localizer PassthroughLocalizer = new Localizer("en-US", () => new List<(string, System.Func<string, string>)>
        {
            ("en-US", key => "{0}"),
        });

        public class PhysboneNotFoundError : SimpleError
        {
            string message;
            public PhysboneNotFoundError(AddPBCollider c) => this.message = $"PhysBone not found at path: [{c.boneReference} / {c.subPath}]";
            public override Localizer Localizer => PassthroughLocalizer;

            public override string TitleKey => "title";
            public override string[] TitleSubst => new string[] { "Error" };
            public override string DetailsKey => "message";
            public override string[] DetailsSubst => new string[] { message };

            public override ErrorSeverity Severity => ErrorSeverity.NonFatal;
        }
    }
}
