using System;
using System.Collections.Generic;
using UnityEngine;
using WithAR20200830.Models;

namespace WithAR20200830.Views
{
	public partial class BoneController : MonoBehaviour
	{
		// 3D joint skeleton
		const int k_NumSkeletonJoints = 91;

		[SerializeField]
		[Tooltip("The root bone of the skeleton.")]
		Transform m_SkeletonRoot;

		Transform[] m_BoneMapping;

		void Awake()
		{
			m_BoneMapping = new Transform[k_NumSkeletonJoints];
		}

		public void InitializeSkeletonJoints()
		{
			// Walk through all the child joints in the skeleton and
			// store the skeleton joints at the corresponding index in the m_BoneMapping array.
			// This assumes that the bones in the skeleton are named as per the
			// JointIndices enum above.
			Queue<Transform> nodes = new Queue<Transform>();
			nodes.Enqueue(m_SkeletonRoot);
			while (nodes.Count > 0)
			{
				Transform next = nodes.Dequeue();
				for (int i = 0; i < next.childCount; ++i)
				{
					nodes.Enqueue(next.GetChild(i));
				}

				ProcessJoint(next);
			}
		}

		public void ApplyBodyPose(DanceFrame bodyFrame)
		{
			for (int i = 0; i < k_NumSkeletonJoints; ++i)
			{
				var bone = m_BoneMapping[i];
				if (bone == null) continue;

				var joint = bodyFrame.Joints[i];
				bone.transform.localPosition = joint.LocalPosePosition;
				bone.transform.localRotation = joint.LocalPoseRotation;
			}
		}

		void ProcessJoint(Transform joint)
		{
			int index = GetJointIndex(joint.name);
			if (index >= 0 && index < k_NumSkeletonJoints)
			{
				m_BoneMapping[index] = joint;
			}
			else
			{
				Debug.LogWarning($"{joint.name} was not found.");
			}
		}

		// Returns the integer value corresponding to the JointIndices enum value
		// passed in as a string.
		int GetJointIndex(string jointName)
		{
			if (Enum.TryParse(jointName, out JointIndices val))
			{
				return (int) val;
			}

			return -1;
		}
	}
}