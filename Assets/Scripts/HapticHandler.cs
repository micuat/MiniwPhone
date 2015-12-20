﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Voronoi;
using Cell = Voronoi.Cell; 

namespace Haptic {
	public class HapticHandler
	{
		public float impactTime = -1;
		List<GameObject> chunks;
		private Bounds bounds;
		private Vector3 impactPoint;
		public int id;

		public HapticHandler(List<GameObject> chunks, Bounds bounds, Vector3 impactPoint, int id, VoronoiDemo voro) {
			impactTime = Time.timeSinceLevelLoad;

			this.chunks = chunks;
			this.bounds = bounds;
			this.impactPoint = impactPoint;
			this.id = id;

			foreach (GameObject chunk in chunks)
			{
                if (chunk == null)
                    continue;
				Cell cell = chunk.GetComponent<FractureChunk>().cell;
				float length = 0.05f;
                //float length = Mathf.Min((Time.timeSinceLevelLoad - impactTime)*2, 2.0f * 2.4f / 6.0f);
                var cellV = cell.site.ToVector3();
                cellV.y = 0;
                bool s = (cellV - impactPoint).magnitude < length;
				if(s) chunk.GetComponent<FractureChunk>().ApplyForce(impactPoint);
			}

			Vector4 impactShader = new Vector4 ();
			impactShader.x = impactPoint.x;
			impactShader.y = impactPoint.y;
			impactShader.z = impactPoint.z;
			impactShader.w = Time.time;
		}
	}
}
