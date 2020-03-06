using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPUCube
{
    public class GPUSharedCube : GPUCubeBase
    {
        public float DistWeight = 10f;
        public float DistWeight2 = 10f;
        [Range(0.0f, 1.0f)]
        public float ColorIntensity = 0.5f;
        [Range(0.0f, 1.0f)]
        public float ColorIntensity2 = 0.5f;
        public float NoiseWeight = 1.0f;

        protected int blockCenterID;
        protected int centerID;

        protected override void Awake()
        {
            base.Awake();
            blockCenterID = GetBlockCenterID();
            centerID = GetCenterCubeID();
        }

        /// <summary>
        /// Execute Init kernel
        /// </summary>
        protected override void ExecuteInitKernel()
        {
            int kernel = CubeCS.FindKernel("Init");

            CubeCS.SetVector("_CubeNumXYZ", CubeNumEachDir);
            CubeCS.SetVector("_PosStepXYZ", PosStep);
            CubeCS.SetVector("_ArangeCenterXYZ", ArangeCenter);
            CubeCS.SetBuffer(kernel, "_CubeBuffer", cubeBuffer);
            CubeCS.Dispatch(kernel, threadGroupSize.x, threadGroupSize.y, threadGroupSize.z);
        }

        /// <summary>
        /// Execute Update kernel
        /// </summary>
        protected override void ExecuteUpdateKernel()
        {
            int kernel = CubeCS.FindKernel("Update");

            CubeCS.SetInt("_BlockCenterID", blockCenterID);
            CubeCS.SetInt("_CenterID", centerID);
            CubeCS.SetFloat("_DistWeight", DistWeight);
            CubeCS.SetFloat("_DistWeight2", DistWeight2);
            CubeCS.SetFloat("_ColorIntensity", ColorIntensity);
            CubeCS.SetFloat("_ColorIntensity2", ColorIntensity2);
            CubeCS.SetFloat("_TimeSpeed", TimeSpeed);
            CubeCS.SetFloat("_DeltaTime", Time.deltaTime);
            CubeCS.SetFloat("_ElapsedTime", elapsedTime);
            CubeCS.SetFloat("_NoiseWeight", NoiseWeight);
            CubeCS.SetVector("_CubeNumXYZ", CubeNumEachDir);
            CubeCS.SetVector("_CubeSizeXYZ", CubeSize);
            CubeCS.SetBuffer(kernel, "_CubeBuffer", cubeBuffer);
            CubeCS.Dispatch(kernel, threadGroupSize.x, threadGroupSize.y, threadGroupSize.z);
        }

        private int GetCenterCubeID()
        {
            int centerXID = (int)CubeNumEachDir.x / 2;
            int centerYID = (int)CubeNumEachDir.y / 2;
            int centerZID = (int)CubeNumEachDir.z / 2;
            int centerID = (int)CubeNumEachDir.x * (int)CubeNumEachDir.y * centerZID +
                           (int)CubeNumEachDir.x * centerYID +
                           centerXID;
            return centerID;
        }

        private int GetBlockCenterID()
        {
            int b_centerXID = threadSize.x / 2;
            int b_centerYID = threadSize.y / 2;
            int b_centerZID = threadSize.z / 2;
            int b_centerID  = threadSize.x * threadSize.y * b_centerZID + 
                              threadSize.x * b_centerYID + 
                              b_centerXID;
            return b_centerID;
        }
    }
}