using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct ParticleSplineSetting
{
	public enum SplineViewedFrom {ThreeD, TwoD}

	private ParticleSystem.Particle[] particles;
	private Vector3[] particleOffsets;

	public ParticleSystem ParticleStream;
	public float StreamWidth;

	[Tooltip("2D will populate the particles in a single plane, 3D will populate them with depth")]
	public SplineViewedFrom ViewedIn;

	public void InitialiseParticleStream()
	{
		ParticleStream.simulationSpace = ParticleSystemSimulationSpace.World;

		if (particles == null || particles.Length < ParticleStream.maxParticles) particles = new ParticleSystem.Particle[ParticleStream.maxParticles]; 

		particleOffsets = new Vector3[particles.Length];

		for (int i = 0; i < particleOffsets.Length; i++)
		{
			if (ViewedIn == SplineViewedFrom.TwoD)
			{
				particleOffsets[i] = Random.insideUnitCircle;
			}
			else
			{
				particleOffsets[i] = Random.insideUnitSphere;
			}
		}
	}

	public void UpdateParticleStream(BezierSpline spline)
	{
		int liveParticles = ParticleStream.GetParticles(particles);

		for (int i = 0; i < liveParticles; i++)
		{
			particles[i].position = spline.GetPoint((particles[i].startLifetime - particles[i].remainingLifetime) / particles[i].startLifetime) + particleOffsets[i] * StreamWidth;
		}

		ParticleStream.SetParticles(particles, liveParticles);
	}
}

[RequireComponent(typeof(BezierSpline))]
public class SplineParticle : MonoBehaviour
{
	private BezierSpline spline;

	private ParticleSystem particleStream;
	private ParticleSystem.Particle[] particles;
	private Vector3[] particleOffsets;

	public ParticleSplineSetting[] ParticleSplines;

	private void Start()
	{
		spline = GetComponent<BezierSpline>();

		for (int i = 0; i < ParticleSplines.Length; i++)
		{
			ParticleSplines[i].InitialiseParticleStream();
		}
	}

	private void LateUpdate()
	{
		for (int i = 0; i < ParticleSplines.Length; i++)
		{
			ParticleSplines[i].UpdateParticleStream(spline);
		}
	}
}