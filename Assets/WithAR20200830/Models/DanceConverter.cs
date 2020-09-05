using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace WithAR20200830.Models
{
	public static class DanceConverter
	{
		public static Dance Trim(this Dance self, float? start, float? end)
		{
			var offset = (int) (start * self.Frames.Count ?? 0);
			var endIndex = (int) (end * self.Frames.Count ?? self.Frames.Count) - 1;
			var length = endIndex - offset;
			var frames = self.Frames.Skip(offset).Take(length).ToArray();

			return new Dance
			{
				Id = Guid.NewGuid(),
				Frames = frames,
			};
		}

		public static DanceFrame CreateFrame(float timestampSecs, ARHumanBody arHumanBody)
		{
			var joints = new List<DanceJoint>();

			foreach (var joint in arHumanBody.joints)
			{
				joints.Add(new DanceJoint
				{
					LocalPosePosition = joint.localPose.position,
					LocalPoseRotation = joint.localPose.rotation,
				});
			}

			return new DanceFrame
			{
				TimestampSecs = timestampSecs,
				Joints = joints,
			};
		}

		public static byte[] SerializeDance(Dance dance)
		{
			using (var stream = new MemoryStream(1024))
			using (var writer = new BinaryWriter(stream))
			{
				writer.Write(dance.Id.ToString("N"));
				writer.Write(dance.Frames.Count);
				foreach (var frame in dance.Frames)
				{
					writer.Write(frame.TimestampSecs);
					writer.Write(frame.Joints.Count);
					foreach (var joint in frame.Joints)
					{
						writer.Write(joint.LocalPosePosition.x);
						writer.Write(joint.LocalPosePosition.y);
						writer.Write(joint.LocalPosePosition.z);
						writer.Write(joint.LocalPoseRotation.w);
						writer.Write(joint.LocalPoseRotation.x);
						writer.Write(joint.LocalPoseRotation.y);
						writer.Write(joint.LocalPoseRotation.z);
					}
				}

				return stream.ToArray();
			}
		}

		public static Dance DeserializeDance(byte[] danceBinary)
		{
			using (var stream = new MemoryStream(danceBinary))
			using (var reader = new BinaryReader(stream))
			{
				var id = reader.ReadString();
				var frameCount = reader.ReadInt32();
				var frames = new DanceFrame[frameCount];

				for (var i = 0; i < frameCount; i++)
				{
					var timestamp = reader.ReadSingle();
					var jointCount = reader.ReadInt32();
					var joints = new DanceJoint[jointCount];

					for (int j = 0; j < jointCount; j++)
					{
						var posX = reader.ReadSingle();
						var posY = reader.ReadSingle();
						var posZ = reader.ReadSingle();
						var rotW = reader.ReadSingle();
						var rotX = reader.ReadSingle();
						var rotY = reader.ReadSingle();
						var rotZ = reader.ReadSingle();

						joints[j] = new DanceJoint
						{
							LocalPosePosition = new Vector3(posX, posY, posZ),
							LocalPoseRotation = new Quaternion(rotX, rotY, rotZ, rotW),
						};
					}

					frames[i] = new DanceFrame
					{
						TimestampSecs = timestamp,
						Joints = joints,
					};
				}

				return new Dance
				{
					Id = Guid.Parse(id),
					Frames = frames,
				};
			}
		}
	}
}