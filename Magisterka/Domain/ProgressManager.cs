﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;

namespace Domain
{
    public class ProgressManager
    {
        #region StepStruct
        internal class Step
        {
            public int MaxSteps { get; set; }
            public int DoneSteps { get; set; }
            public double Percent { get; set; }
        }
        #endregion StepStruct

        #region EventProgressStatusChanged
        public delegate void RefreshProgress(int progress);
        public static event RefreshProgress ProgressStatusChanged;
        #endregion EventProgressStatusChanged

        #region PublicFields
        public const int MaxValueProgressBar = 1000;
        public static int ProgressPercent
        {
            get
            {
                return _steps == null ? 0 : (int)((_progress * 100) / MaxValueProgressBar);
            }
        }
        public static double Progress
        {
            get
            {
                return _progress < MaxValueProgressBar ? _progress : MaxValueProgressBar;
            }
            set
            {
                if (value != _progress)
                {
                    if((int)value>(int)_progress)
                    {
                        ProgressStatusChanged?.Invoke((int)Progress);
                    }                   
                    _progress = value;                            
                }
            }
        }
        #endregion PublicFields

        #region PrivateFields
        private static List<Step> _steps;
        private static double _progress = 0.0;
        #endregion PrivateFields

        #region Methods
        public static void AddSteps(int stepsCount)
        {
            Step step = new Step();
            step.MaxSteps = stepsCount;
            step.DoneSteps = 0;
            double percent = _steps?.Count() > 0 ? _steps.Last().Percent : MaxValueProgressBar;
            step.Percent = ((percent) / (step.MaxSteps > 0 ? step.MaxSteps : 1));

            if (_steps == null) _steps = new List<Step>();
            _steps.Add(step);
        }
        public static void DoStep(bool sum=true)
        {
            if (_steps?.Count > 0)
            {
                int inx = _steps.Count() - 1;
                _steps[inx].DoneSteps = _steps[inx].DoneSteps + 1;
                if (sum)
                {
                    Progress += _steps[inx].Percent;
                }

                if (_steps[inx].DoneSteps == _steps[inx].MaxSteps)
                {                    
                    _steps.RemoveAt(inx);
                    DoStep(false);                                      
                    if (inx == 0 || Progress >= MaxValueProgressBar)
                    {
                        Progress = MaxValueProgressBar;
                        return;
                    }                   
                }
            }
        }
        public static void FinallySteps()
        {
            Progress = MaxValueProgressBar;
            ClearSteps();
        }
        public static void ClearSteps()
        {
            _steps = null;
            Progress = 0;
        }
        #endregion Methods
    }
}
