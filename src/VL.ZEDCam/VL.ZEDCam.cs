using sl;
using System;
using System.Collections.Generic;
using System.Threading;

namespace sl
{
	public class ZEDCam
	{
		Camera camera;
		InitParameters initParameters;
		object grabLock = new object();

		ERROR_CODE lastInitStatus = ERROR_CODE.ERROR_CODE_LAST;
		ERROR_CODE previousInitStatus;


		private bool running;
		public bool Running
		{
			get => running;
		}

		List<Mat> mats;
		int matIndex;

		private RuntimeParameters runtimeParameters;
		public RuntimeParameters RuntimeParameters
		{
			get { return this.runtimeParameters; }
			set { this.runtimeParameters = value; }
		}

        public float FOV_H
        {
            get { return this.camera.HorizontalFieldOfView; }
            //get { return this.camera.CalibrationParametersRectified.leftCam.hFOV; }
        }

        public float FOV_V
        {
            get { return this.camera.VerticalFieldOfView; }
            //get { return this.camera.CalibrationParametersRectified.leftCam.vFOV; }
        }

        public int FPS
        {
            get { return (int)this.camera.GetCameraFPS(); }
            set { this.camera.SetCameraFPS(value); }
        }

        public void setFPS (fps x)
        {
            int t = 30;
            switch(x)
            {
                case fps._15FPS: t = 15; break;
                case fps._30FPS: t = 30; break;
                case fps._60FPS: t = 60; break;
                case fps._100FPS:t = 100; break;
            }
            FPS = t;
        }

        // constructor
        public ZEDCam() : this(null, null) { }

		public ZEDCam(string svoPath) : this(null, svoPath) { }

		public ZEDCam(InitParameters parameters = null, string svoPath = null)
		{
			
            
            mats = new List<Mat>();

			if (parameters == null)
			{
				parameters = new InitParameters();
				parameters.resolution = RESOLUTION.HD720;
				parameters.depthMode = DEPTH_MODE.QUALITY;
				parameters.depthStabilization = true;
				parameters.enableRightSideMeasure = true; // isStereoRig;

				parameters.coordinateUnits = UNIT.MILLIMETER;
				parameters.depthMinimumDistance = 200f;
            }

			if (svoPath != null)
			{
				parameters.pathSVO = svoPath;
			}

			this.initParameters = parameters;

			// runtime parameters
			runtimeParameters = new RuntimeParameters()
			{
				sensingMode = SENSING_MODE.FILL,
				enableDepth = true
			};

			// create the camera
			camera = new Camera(1);
		}

		public ZEDCam(RESOLUTION resolution, DEPTH_MODE depthMode = DEPTH_MODE.QUALITY, bool stabilisation = true)
			: this(new InitParameters
			{
				resolution = resolution,
				depthMode = depthMode,
				depthStabilization = stabilisation,
				enableRightSideMeasure = true,

				coordinateUnits = UNIT.MILLIMETER,
				depthMinimumDistance = 200f
			})
		{ }

		private Mat CreateViewMaterial(Mat mat, VIEW view)
        {
			uint w = (uint)(view == VIEW.SIDE_BY_SIDE ? 2 * camera.ImageWidth : camera.ImageWidth);
			uint h = (uint)camera.ImageHeight;
			MAT_TYPE type = view.GetMatType();
			mat.Create(w, h, type, MEM.CPU);

			var cb = type.GetChannelAndByts();
			var c = mat.GetChannels();
			var p = mat.GetPixelBytes();

			return mat;
		}
		private Mat CreateMeasureMaterial(Mat mat, MEASURE measure)
		{
			uint w = (uint)camera.ImageWidth;
			uint h = (uint)camera.ImageHeight;
			MAT_TYPE type = measure.GetMatType();
			mat.Create(w, h, type, MEM.CPU);
			return mat;
		}

		public void GetResolution(out int Width, out int Height)
        {
			Width = camera.ImageWidth;
			Height = camera.ImageHeight;
		}

		public Mat RetrieveImage(Mat mat, VIEW view)
        {
			if(mat.MatPtr == IntPtr.Zero)
			{
				mat = CreateViewMaterial(mat, view);
			}
			else
            {
				var cb = view.GetMatType().GetChannelAndByts();		
				int w = (view == VIEW.SIDE_BY_SIDE ? 2 * camera.ImageWidth : camera.ImageWidth);
				int h = camera.ImageHeight;

				var c = mat.GetChannels();
				var p = mat.GetPixelBytes();


				if (mat.GetChannels() != cb.channel || mat.GetPixelBytes() != cb.bytes || mat.GetWidth() != w || mat.GetHeight() != h)
				{
					mat.Free();
					mat = CreateViewMaterial(mat, view);
				}	
			}
			var ERR = camera.RetrieveImage(mat, view);

			var c1 = mat.GetChannels();
			var p1 = mat.GetPixelBytes();

			return mat;
        }

        public Mat RetrieveMeasure(Mat mat, MEASURE measure)
        {
			if (mat.MatPtr == IntPtr.Zero)
			{
				mat = CreateMeasureMaterial(mat, measure);
			}
			else
			{
				var cb = measure.GetMatType().GetChannelAndByts();
				if (mat.GetChannels() != cb.channel || mat.GetPixelBytes() != cb.bytes || mat.GetWidth() != camera.ImageWidth || mat.GetHeight() != camera.ImageHeight)
				{
					mat.Free();
					mat = CreateMeasureMaterial(mat, measure);
				}
			}
			camera.RetrieveMeasure(mat, measure);
            return mat;
        }

        public void InitZED_Sequential()
		{
			while (lastInitStatus != sl.ERROR_CODE.SUCCESS)
			{
				lastInitStatus = camera.Open(ref initParameters);
                if (lastInitStatus != sl.ERROR_CODE.SUCCESS)
				{
					lastInitStatus = camera.Open(ref initParameters);
					previousInitStatus = lastInitStatus;
				}
				Thread.Sleep(300);
			}
        }

		public void Start_Sequential()
		{
			if (running == true) return;
			InitZED_Sequential();
			running = true;
		}

		public void GrabOnce()
		{
			if (running)
			{
				lock (grabLock)
				{
					sl.ERROR_CODE e = camera.Grab(ref runtimeParameters);
					// reset mat index
					matIndex = 0;
				}
			}
			else
			{
				//Console.WriteLine("Not open");
			}
		}

		public void Stop()
		{
			this.running = false;
		}

		public void Close()
		{
            camera.Close();
		}

		public void TT()
		{
			//camera.SetCameraSettings();
		}

        public enum fps
        {
            /// <summary>
            /// 2208*1242. Supported frame rate: 15 FPS.
            /// </summary>
            _15FPS,
            /// <summary>
            /// 1920*1080. Supported frame rates: 15, 30 FPS.
            /// </summary>
            _30FPS,
            /// <summary>
            /// 1280*720. Supported frame rates: 15, 30, 60 FPS.
            /// </summary>
            _60FPS,
            /// <summary>
            /// 672*376. Supported frame rates: 15, 30, 60, 100 FPS.
            /// </summary>
            _100FPS
        };
    }
}
