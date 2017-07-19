using System;
using Expanse.Misc;
using UnityEngine;

namespace Expanse.Replay
{
    public sealed class ReplayTransform : MonoBehaviour
    {
        private Transform Transform;

        private ReplayManager replayManager;

        public int majorStepLength = 10;
        public int minorStepLength = 2;
        public bool recordParentChanges = false;
        public PositionRecordType positionRecordType = PositionRecordType.LocalXYZ;
        public RotationRecordType rotationRecordType = RotationRecordType.LocalXYZ;
        public ScaleRecordType scaleRecordType = ScaleRecordType.None;

#pragma warning disable 414
        private StepTransformParentInfo[] transformParentData;
        private int transformParentIndex;
        private Array majorPositionData, minorPositionData;
        private int majorPositionIndex, minorPositionIndex;
        private Array majorRotationData, minorRotationData;
        private int majorRotationIndex, minorRotationIndex;
        private Array majorScaleData, minorScaleData;
        private int majorScaleIndex, minorScaleIndex;
#pragma warning restore 414

        private int frameCount;
        private float currentTime;
        private int majorDataLength;
        private int minorDataLength;

        void Start()
        {
            Transform = transform;

            replayManager = ReplayManager.Instance;

            currentTime = TimeManager.Time;

            float framesPerSecond = 1f / TimeManager.FixedDeltaTime;
            int maxFrames = Mathf.CeilToInt(replayManager.replayDuration * framesPerSecond);

            majorDataLength = maxFrames / majorStepLength;
            minorDataLength = maxFrames / minorStepLength;

            if (recordParentChanges)
            {
                transformParentData = new StepTransformParentInfo[minorDataLength];
            }

            switch (positionRecordType)
            {
                case PositionRecordType.XYZ:
                case PositionRecordType.LocalXYZ:
                    majorPositionData = new MajorStepVector3Info[majorDataLength];
                    minorPositionData = new MinorStepVector3Info[minorDataLength];
                    break;

                case PositionRecordType.XY:
                case PositionRecordType.LocalXY:
                case PositionRecordType.XZ:
                case PositionRecordType.LocalXZ:
                    majorPositionData = new MajorStepVector2Info[majorDataLength];
                    minorPositionData = new MinorStepVector2Info[minorDataLength];
                    break;
            }

            switch (rotationRecordType)
            {
                case RotationRecordType.XYZ:
                case RotationRecordType.LocalXYZ:
                    majorRotationData = new MajorStepVector3Info[majorDataLength];
                    minorRotationData = new MinorStepVector3Info[minorDataLength];
                    break;

                case RotationRecordType.Z:
                case RotationRecordType.LocalZ:
                    majorRotationData = new MajorStepVector1Info[majorDataLength];
                    minorRotationData = new MinorStepVector1Info[minorDataLength];
                    break;
            }

            switch (scaleRecordType)
            {
                case ScaleRecordType.LocalXYZ:
                    majorScaleData = new MajorStepVector3Info[majorDataLength];
                    minorScaleData = new MinorStepVector3Info[minorDataLength];
                    break;
            }
        }

        void FixedUpdate()
        {
            if (frameCount % majorStepLength == 0)
            {
                RecordMajorStep();
            }
            else if (frameCount % minorStepLength == 0)
            {
                RecordMinorStep();
            }

            currentTime += TimeManager.FixedDeltaTime;
            frameCount++;
        }

        private void RecordMajorStep()
        {

        }

        private void RecordMinorStep()
        {
            // Record transform parent

            if (recordParentChanges)
            {
                transformParentData[transformParentIndex] = new StepTransformParentInfo(currentTime, Transform.parent);

                transformParentIndex++;

                if (transformParentIndex >= minorDataLength)
                    transformParentIndex = 0;
            }

            // Record minor position

            bool recordedPosition = true;
            switch (positionRecordType)
            {
                case PositionRecordType.XYZ:
                    {
                        Vector3 position = Transform.position;
                        (minorPositionData as MinorStepVector3Info[])[minorPositionIndex] = new MinorStepVector3Info();
                        break;
                    }
                case PositionRecordType.LocalXYZ:
                    break;
                case PositionRecordType.XY:
                    break;
                case PositionRecordType.LocalXY:
                    break;
                case PositionRecordType.XZ:
                    break;
                case PositionRecordType.LocalXZ:
                    break;
                default:
                    recordedPosition = false;
                    break;
            }

            if (recordedPosition)
            {
                minorPositionIndex++;

                if (minorPositionIndex >= minorDataLength)
                    minorPositionIndex = 0;
            }
        }

        public enum PositionRecordType
        {
            None = 0,
            XYZ = 1,
            LocalXYZ = 2,
            XY = 3,
            LocalXY = 4,
            XZ = 5,
            LocalXZ = 6
        }

        public enum RotationRecordType
        {
            None = 0,
            XYZ = 1,
            LocalXYZ = 2,
            Z = 3,
            LocalZ = 4
        }

        public enum ScaleRecordType
        {
            None = 0,
            LocalXYZ = 1
        }

        private struct StepTransformParentInfo
        {
            public float time;
            public Transform parent;

            public StepTransformParentInfo(float time, Transform parent)
            {
                this.time = time;
                this.parent = parent;
            }
        }

        private struct MajorStepVector3Info
        {
            public float time;
            public Vector3 value;

            public MajorStepVector3Info(float time, Vector3 value)
            {
                this.time = time;
                this.value = value;
            }
        }

        private struct MinorStepVector3Info
        {
            public Half deltaTime;
            public Half deltaValueX;
            public Half deltaValueY;
            public Half deltaValueZ;

            public MinorStepVector3Info(Half deltaTime, Half deltaValueX, Half deltaValueY, Half deltaValueZ)
            {
                this.deltaTime = deltaTime;
                this.deltaValueX = deltaValueX;
                this.deltaValueY = deltaValueY;
                this.deltaValueZ = deltaValueZ;
            }
        }

        private struct MajorStepVector2Info
        {
            public float time;
            public Vector2 value;

            public MajorStepVector2Info(float time, Vector2 value)
            {
                this.time = time;
                this.value = value;
            }
        }

        private struct MinorStepVector2Info
        {
            public Half deltaTime;
            public Half deltaValueX;
            public Half deltaValueY;

            public MinorStepVector2Info(Half deltaTime, Half deltaValueX, Half deltaValueY)
            {
                this.deltaTime = deltaTime;
                this.deltaValueX = deltaValueX;
                this.deltaValueY = deltaValueY;
            }
        }

        private struct MajorStepVector1Info
        {
            public float time;
            public float value;

            public MajorStepVector1Info(float time, float value)
            {
                this.time = time;
                this.value = value;
            }
        }

        private struct MinorStepVector1Info
        {
            public Half deltaTime;
            public Half deltaValue;

            public MinorStepVector1Info(Half deltaTime, Half deltaValue)
            {
                this.deltaTime = deltaTime;
                this.deltaValue = deltaValue;
            }
        }
    }
}
