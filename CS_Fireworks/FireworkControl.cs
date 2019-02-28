//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CS_Fireworks
{
    class FireworkControl : MonoBehaviour
    {
        public ParticleSystem particle;
        public ParticleSystem.MainModule mainm;
        public ParticleSystem.EmissionModule emitm;
        public FireworkControlMode mode = FireworkControlMode.Once;

        public float looplength = 5;

        public float possibility = 0.005f;

        /// <summary>
        /// t∈[0,1]
        /// </summary>
        /// <param name="t"></param>
        public void SetT(float t)
        {
            particle.time = t * particle.main.duration;
        }

        private void Start()
        {
            if (particle == null)
            {
                particle = GetComponent<ParticleSystem>();
            }
            mainm = particle.main;
            emitm = particle.emission;
            switch (mode)
            {
                case FireworkControlMode.Once:
                    mainm.loop = false;
                    emitm.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0, 1) });
                    particle.Play();
                    break;
                case FireworkControlMode.Loop:
                    mainm.loop = true;
                    emitm.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0, 1) });
                    mainm.duration = looplength;
                    particle.Play();
                    break;
                case FireworkControlMode.Random:
                    mainm.loop = true;
                    emitm.SetBursts(new ParticleSystem.Burst[0]);
                    particle.Play();
                    break;
            }
        }

        private void Update()
        {
            switch (mode)
            {
                case FireworkControlMode.Once:
                    if (particle.isStopped)
                    {
                        Destroy(gameObject);
                        FireworksManager.LogMsg("particle destroyd");
                    }
                    break;
                case FireworkControlMode.Loop:
                    if (mainm.duration != looplength)
                    {
                        mainm.duration = looplength;
                    }
                    break;
                case FireworkControlMode.Random:
                    if (Random.value < possibility)
                    {
                        particle.Emit(1);
                    }
                    break;
            }

            if (mainm.simulationSpeed != SimulationManager.instance.FinalSimulationSpeed && SimulationManager.instance.FinalSimulationSpeed != 0)
            {
                //mainm.simulationSpeed = SimulationManager.instance.FinalSimulationSpeed;
                SetSpeed(particle, SimulationManager.instance.FinalSimulationSpeed);
            }
            if ((SimulationManager.instance.SimulationPaused || SimulationManager.instance.FinalSimulationSpeed == 0) && particle.isPlaying)
            {
                particle.Pause();
            }
            else if (!(SimulationManager.instance.SimulationPaused || SimulationManager.instance.FinalSimulationSpeed == 0) && particle.isPaused)
            {
                particle.Play();
            }
        }

        public static void SetSpeed(ParticleSystem ps, float speed)
        {
            ParticleSystem.MainModule main_module = ps.main;
            main_module.simulationSpeed = speed;
            ParticleSystem.SubEmittersModule sub_module = ps.subEmitters;
            if (sub_module.enabled)
            {
                for (int sub_i = 0; sub_i < sub_module.subEmittersCount; sub_i++)
                {
                    ParticleSystem sub_sys = sub_module.GetSubEmitterSystem(sub_i);
                    SetSpeed(sub_sys, speed);
                }
            }
        }

        public static void MulSize(ParticleSystem ps, float scale)
        {
            ParticleSystem.MainModule ps_main = ps.main;
            ps_main.startSize = ScaleCurve(ps_main.startSize, scale);

            ParticleSystem.SubEmittersModule ps_sub = ps.subEmitters;
            if (ps_sub.enabled)
            {
                for(int si = 0; si < ps_sub.subEmittersCount; si++)
                {
                    MulSize(ps_sub.GetSubEmitterSystem(si), scale);
                }
            }
        }

        public static void MulExpVel(ParticleSystem ps, float scale)
        {
            MulExpVel(ps, scale, true, true);
        }
        public static void MulExpVel(ParticleSystem ps, float scale, bool self, bool child)
        {
            if (self)
            {
                ParticleSystem.MainModule ps_main = ps.main;
                ps_main.startSpeed = ScaleCurve(ps_main.startSpeed, scale);
            }

            if (child)
            {
                ParticleSystem.SubEmittersModule ps_sub = ps.subEmitters;
                if (ps_sub.enabled)
                {
                    for (int si = 0; si < ps_sub.subEmittersCount; si++)
                    {
                        MulExpVel(ps_sub.GetSubEmitterSystem(si), scale);
                    }
                }
            }
        }

        public static ParticleSystem.MinMaxCurve ScaleCurve(ParticleSystem.MinMaxCurve curve, float scale)
        {
            switch (curve.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    curve.constant *= scale;
                    break;
                case ParticleSystemCurveMode.TwoConstants:
                    curve.constantMin *= scale;
                    curve.constantMax *= scale;
                    break;
                case ParticleSystemCurveMode.Curve:
                    for (int ki = 0; ki < curve.curve.length; ki++)
                    {
                        curve.curve.keys[ki].value *= scale;
                    }
                    break;
                case ParticleSystemCurveMode.TwoCurves:
                    for (int ki = 0; ki < curve.curveMin.length; ki++)
                    {
                        curve.curveMin.keys[ki].value *= scale;
                    }
                    for (int ki = 0; ki < curve.curveMax.length; ki++)
                    {
                        curve.curveMax.keys[ki].value *= scale;
                    }
                    break;
            }
            return curve;
        }
    }

    enum FireworkControlMode
    {
        Once,
        Loop,
        Random,
    }
}
