using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPUCube
{
    public class GPUCube2D : GPUCubeBase
    {
        public float WaveHeight = 5f;

        /// <summary>
        /// Execute Init kernel
        /// </summary>
        protected override void ExecuteInitKernel()
        {
            int kernel = CubeCS.FindKernel("Init");

            // 注意点
            // SetVectorで渡す変数の型を間違えないように。
            // Vector3はFloat型になるから、基本GPU側で受け取る変数の型もfloat3とかにしておく
            CubeCS.SetVector("_CubeNumXY", CubeNumEachDir);
            CubeCS.SetVector("_PosStepXY", PosStep);
            CubeCS.SetVector("_ArangeCenterXY", ArangeCenter);
            CubeCS.SetBuffer(kernel, "_CubeBuffer", cubeBuffer);
            CubeCS.Dispatch(kernel, threadGroupSize.x, threadGroupSize.y, threadGroupSize.z);
        }

        /// <summary>
        /// Execute Update kernel
        /// </summary>
        protected override void ExecuteUpdateKernel()
        {
            int kernel = CubeCS.FindKernel("Update");

            CubeCS.SetFloat("_TimeSpeed", TimeSpeed);
            CubeCS.SetFloat("_ElapsedTime", elapsedTime);
            CubeCS.SetFloat("_WaveHeight", WaveHeight);
            CubeCS.SetVector("_CubeNumXY", CubeNumEachDir);
            CubeCS.SetVector("_CubeSizeXY", CubeSize);
            CubeCS.SetBuffer(kernel, "_CubeBuffer", cubeBuffer);
            CubeCS.Dispatch(kernel, threadGroupSize.x, threadGroupSize.y, threadGroupSize.z);
        }
    }
}