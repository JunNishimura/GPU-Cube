using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPUCube
{
    public abstract class GPUCubeBase : MonoBehaviour
    {
        [System.Serializable]
        public struct Cube
        {
            public Vector3 position;
            public Vector3 size;
            public Color color;
        }

        public ComputeShader CubeCS;
        public Vector3 CubeNumEachDir = new Vector3(64, 64, 64);
        public Vector3 CubeSize = Vector3.one;
        public Vector3 PosStep = new Vector3(2f, 2f, 2f);
        public float TimeSpeed = 1f;

        protected ComputeBuffer cubeBuffer;
        protected Vector3Int threadGroupSize = Vector3Int.one;
        protected Vector3Int threadSize      = Vector3Int.one;
        protected float elapsedTime = 0f;

        protected int TotalCubeNum
        {
            get
            {
                return (int)CubeNumEachDir.x * (int)CubeNumEachDir.y * (int)CubeNumEachDir.z;
            }
        }
        protected Vector3 ArangeCenter
        {
            get
            {
                return new Vector3
                (
                    (CubeNumEachDir.x - 1) / 2,
                    (CubeNumEachDir.y - 1) / 2,
                    (CubeNumEachDir.z - 1) / 2
                );
            }
        }


        public int GetTotalCubeNum()
        {
            return TotalCubeNum;
        }

        public ComputeBuffer GetCubeBuffer()
        {
            return cubeBuffer != null ? cubeBuffer : null;
        }

        protected virtual void Awake()
        {
            SetThreadAndThreadGroupSize();
            InitBuffer();
            ExecuteInitKernel();
        }

        protected virtual void Update()
        {
            ExecuteUpdateKernel();

            elapsedTime += Time.deltaTime;
        }

        private void OnDisable()
        {
            ReleaseBuffer();
        }

        protected abstract void ExecuteInitKernel();
        protected abstract void ExecuteUpdateKernel();

        /// <summary>
        /// Initialize ComputeBuffer
        /// </summary>
        protected virtual void InitBuffer()
        {
            cubeBuffer = new ComputeBuffer(TotalCubeNum, Marshal.SizeOf(typeof(Cube)));
            var cubes = CreateCubes();
            cubeBuffer.SetData(cubes);
        }

        /// <summary>
        /// Set number of threads and threadGroups
        /// </summary>
        private void SetThreadAndThreadGroupSize()
        {
            GetKernelThreadSize(out threadSize, "Init");
            threadGroupSize = new Vector3Int
            (
                Mathf.CeilToInt(CubeNumEachDir.x / threadSize.x),
                Mathf.CeilToInt(CubeNumEachDir.y / threadSize.y),
                Mathf.CeilToInt(CubeNumEachDir.z / threadSize.z)
            );
        }

        /// <summary>
        /// Get number of threads at designated kernel
        /// </summary>
        /// <param name="threadSize"></param>
        /// <param name="kernelName"></param>
        private void GetKernelThreadSize(out Vector3Int threadSize, string kernelName)
        {
            uint threadSizeX, threadSizeY, threadSizeZ;

            int kernel = CubeCS.FindKernel(kernelName);
            CubeCS.GetKernelThreadGroupSizes(kernel, out threadSizeX, out threadSizeY, out threadSizeZ);
            threadSize = new Vector3Int((int)threadSizeX, (int)threadSizeY, (int)threadSizeZ);
        }

        /// <summary>
        /// Main cube init setting will be done by GPU
        /// </summary>
        /// <returns>cubes</returns>
        private Cube[] CreateCubes()
        {
            var cubes = new Cube[TotalCubeNum];

            // ----- arange cubes by GPU ----- //
            // just initialize here
            for (int i = 0; i < TotalCubeNum; i++)
            {
                cubes[i].position = Vector3.zero;
                cubes[i].size = CubeSize;
                cubes[i].color = Color.white;
            }
            return cubes;
        }

        /// <summary>
        /// Release used buffer
        /// </summary>
        private void ReleaseBuffer()
        {
            if (cubeBuffer != null)
                cubeBuffer.Release();
            cubeBuffer = null;
        }
    }
}