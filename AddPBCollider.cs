using nadena.dev.ndmf.runtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;
using VRC.SDKBase;

namespace Narazaka.VRChat.AddPBCollider
{
    public class AddPBCollider : MonoBehaviour, IEditorOnly
    {
        public VRCPhysBone targetPb
        {
            get
            {
                var avatarRoot = GetAvatarRoot();
                var animator = avatarRoot.GetComponent<Animator>();
                var boneTransform = GetBoneTransform(avatarRoot);
                if (boneTransform == null) return null;
                var t = GetSubTransform(boneTransform);
                if (t == null) return null;
                var physBones = t.GetComponents<VRCPhysBone>();
                if (pbIndex < 0 || physBones.Length <= pbIndex) return null;
                return physBones[pbIndex];
            }
            set
            {
                if (value == null)
                {
                    boneReference = HumanBodyBones.LastBone;
                    subPath = null;
                    pbIndex = 0;
                    return;
                }

                var t = value.transform;
                var avatarRoot = GetAvatarRoot();
                var animator = avatarRoot.GetComponent<Animator>();
                var bones = new Dictionary<Transform, HumanBodyBones>();

                foreach (var bone in Enum.GetValues(typeof(HumanBodyBones)))
                {
                    if ((HumanBodyBones)bone == HumanBodyBones.LastBone) continue;
                    var boneTransform = animator.GetBoneTransform((HumanBodyBones)bone);
                    if (boneTransform != null)
                    {
                        bones[boneTransform] = (HumanBodyBones)bone;
                    }
                }

                var parent = t;
                while (parent != null && parent != avatarRoot)
                {
                    if (bones.TryGetValue(parent, out var bone))
                    {
                        boneReference = bone;
                        subPath = RuntimeUtil.RelativePath(parent, t);
                        pbIndex = Array.IndexOf(t.GetComponents<VRCPhysBone>(), value);
                        return;
                    }
                    parent = parent.parent;
                }
            }
        }

        public HumanBodyBones boneReference = HumanBodyBones.LastBone;
        Transform GetBoneTransform(Transform root)
        {
            if (boneReference == HumanBodyBones.LastBone)
            {
                return root;
            }

            var animator = root.GetComponent<Animator>();
            if (animator == null || !animator.isHuman)
            {
                return null;
            }

            return animator.GetBoneTransform(boneReference);
        }

        public string subPath;
        Transform GetSubTransform(Transform root) => subPath != null && subPath.Length > 0 ? root.Find(subPath) : root;

        public int pbIndex;

        public VRCPhysBoneCollider[] colliders;

        Transform GetAvatarRoot() => RuntimeUtil.FindAvatarInParents(transform);
    }
}
