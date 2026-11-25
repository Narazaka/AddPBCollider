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
        public override string DisplayName => nameof(AddPBCollider);

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
            ("en-US", key =>
            {
                switch (key)
                {
                    case "pbNotFound": return "PhysBone not found ?";
                    case "pbNotFound:description": return "{1} th PhysBone of [{0}]";
                    default: return key;
                }
            }
            ),
            ("ja-JP", key =>
            {
                switch (key)
                {
                    case "pbNotFound": return "PhysBone が見付かりません？";
                    case "pbNotFound:description": return "[{0}] の {1} 番目の PhysBone";
                    default: return key;
                }
            }
            ),
        });

        public class PhysboneNotFoundError : SimpleError
        {
            HumanBodyBones boneReference;
            string subPath;
            int pbIndex;
            public PhysboneNotFoundError(AddPBCollider c)
            {
                boneReference = c.boneReference;
                subPath = c.subPath;
                pbIndex = c.pbIndex;
            }
            public override Localizer Localizer => PassthroughLocalizer;

            public override string TitleKey => "pbNotFound";
            public override string[] DetailsSubst => new string[] { boneReference == HumanBodyBones.LastBone ? $"<AvatarRoot>/{subPath}" : $"<{boneReference}>/{subPath}", pbIndex.ToString() };

            public override ErrorSeverity Severity => ErrorSeverity.Information;
        }
    }
}
