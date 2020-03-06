using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPUCube
{
    public class CubeInstancingRender : MonoBehaviour
    {
        public GPUCubeBase GPUCubeScript;
        public Mesh InstanceMesh;
        public Material InstanceMaterial;
        protected int InstanceCount = 10000;
        protected int SubmeshIndex = 0;
        protected Bounds InstancingBounds = new Bounds(Vector3.zero, new Vector3(1000f, 1000f, 1000f));

        private ComputeBuffer argsBuffer;
        // parametes for GPU Instancing
        // 1. index count per instance
        // 2. instance count 
        // 3. start index location 
        // 4. base vertex location 
        // 5. start instance location
        private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

        private void Start()
        {
            argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        }

        private void Update()
        {
            InstancingRender();
        }
        private void OnDisable()
        {
            ReleaseBuffer();
        }

        protected virtual void InstancingRender()
        {
            if (InstanceMesh == null || InstanceMaterial == null || !SystemInfo.supportsInstancing)
                return;

            args[0] = (uint)InstanceMesh.GetIndexCount(0);
            args[1] = (uint)GPUCubeScript.GetTotalCubeNum();
            args[2] = (uint)InstanceMesh.GetIndexStart(SubmeshIndex);
            args[3] = (uint)InstanceMesh.GetBaseVertex(SubmeshIndex);
            argsBuffer.SetData(args);
            InstanceMaterial.SetBuffer("_CubeBuffer", GPUCubeScript.GetCubeBuffer());

            Graphics.DrawMeshInstancedIndirect(InstanceMesh, SubmeshIndex, InstanceMaterial, InstancingBounds, argsBuffer);
        }

        private void ReleaseBuffer()
        {
            if (argsBuffer != null)
                argsBuffer.Release();
            argsBuffer = null;
        }
    }
}