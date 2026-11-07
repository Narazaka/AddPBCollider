using NUnit.Framework;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace Narazaka.VRChat.AddPBCollider.Tests
{
    public class AddPBColliderTest
    {
        GameObject avatar;
        AddPBCollider component;

        [SetUp]
        public void SetUp()
        {
            avatar = new GameObject("Avatar");
            avatar.AddComponent<VRCAvatarDescriptor>();
            component = avatar.AddComponent<AddPBCollider>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(avatar);
        }

        [Test]
        public void NewComponentDoesNotSelectRootPhysBone()
        {
            avatar.AddComponent<VRCPhysBone>();
            Assert.That(component.targetPb, Is.Null);
        }

        [Test]
        public void ClearingTargetDoesNotSelectRootPhysBone()
        {
            var rootPb = avatar.AddComponent<VRCPhysBone>();
            component.targetPb = rootPb;
            Assert.That(component.targetPb, Is.SameAs(rootPb));
            component.targetPb = null;
            Assert.That(component.targetPb, Is.Null);
        }

        [Test]
        public void SelectsPhysBoneOutsideHumanoidHierarchy()
        {
            var child = new GameObject("Accessories");
            child.transform.SetParent(avatar.transform);
            var pb = child.AddComponent<VRCPhysBone>();
            component.targetPb = pb;
            Assert.That(component.targetPb, Is.SameAs(pb));
        }

        [Test]
        public void SelectsPhysBoneWhenAnimatorIsNotHumanoid()
        {
            avatar.AddComponent<Animator>();
            var child = new GameObject("Tail");
            child.transform.SetParent(avatar.transform);
            var pb = child.AddComponent<VRCPhysBone>();
            component.targetPb = pb;
            Assert.That(component.targetPb, Is.SameAs(pb));
        }

        [Test]
        public void InspectorCanResolveTargetOutsideAvatar()
        {
            var rootPb = avatar.AddComponent<VRCPhysBone>();
            component.targetPb = rootPb;
            Object.DestroyImmediate(avatar.GetComponent<VRCAvatarDescriptor>());
            Assert.That(component.targetPb, Is.Null);
            Assert.DoesNotThrow(() => component.targetPb = rootPb);
        }
    }
}
