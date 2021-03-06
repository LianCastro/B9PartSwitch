﻿using System;
using UnityEngine;

namespace B9PartSwitch.PartSwitch.PartModifiers
{
    public class AttachNodeMover : PartModifierBase
    {
        public readonly AttachNode attachNode;
        private readonly Vector3 position;
        private readonly ILinearScaleProvider linearScaleProvider;

        public AttachNodeMover(AttachNode attachNode, Vector3 position, ILinearScaleProvider linearScaleProvider)
        {
            attachNode.ThrowIfNullArgument(nameof(attachNode));
            linearScaleProvider.ThrowIfNullArgument(nameof(linearScaleProvider));

            this.attachNode = attachNode;
            this.position = position;
            this.linearScaleProvider = linearScaleProvider;
        }

        public override object PartAspectLock => attachNode.id + "---position";
        public override string Description => $"attach node '{attachNode.id}'";

        public override void ActivateOnStartEditor() => SetAttachNodePosition();
        public override void ActivateOnStartFlight() => SetAttachNodePosition();

        // TweakScale resets node positions, therefore we need to wait a frame and fix them
        public override void ActivateAfterStart() => SetAttachNodePosition();

        public override void ActivateOnSwitchEditor() => SetAttachNodePositionAndMoveParts();
        public override void ActivateOnSwitchFlight() => SetAttachNodePosition();
        public override void DeactivateOnSwitchEditor() => UnsetAttachNodePositionAndMoveParts();
        public override void DeactivateOnSwitchFlight() => UnsetAttachNodePosition();

        private void SetAttachNodePositionAndMoveParts()
        {
            Vector3 offset = (position * linearScaleProvider.LinearScale) - attachNode.position;
            SetAttachNodePosition();

            if (!HighLogic.LoadedSceneIsEditor) return;
            if (attachNode.owner.parent != null && attachNode.owner.parent == attachNode.attachedPart)
            {
                attachNode.owner.transform.localPosition -= offset;
            }
            else if (attachNode.attachedPart != null)
            {
                attachNode.attachedPart.transform.localPosition += offset;
            }
        }

        private void UnsetAttachNodePositionAndMoveParts()
        {
            Vector3 offset = attachNode.position - (position * linearScaleProvider.LinearScale);
            UnsetAttachNodePosition();

            if (!HighLogic.LoadedSceneIsEditor) return;
            if (attachNode.owner.parent != null && attachNode.owner.parent == attachNode.attachedPart)
            {
                attachNode.owner.transform.localPosition -= offset;
            }
            else if (attachNode.attachedPart != null)
            {
                attachNode.attachedPart.transform.localPosition += offset;
            }
        }

        private void SetAttachNodePosition()
        {
            attachNode.position = position * linearScaleProvider.LinearScale;
        }

        private void UnsetAttachNodePosition()
        {
            attachNode.position = attachNode.originalPosition * linearScaleProvider.LinearScale;
        }
    }
}
