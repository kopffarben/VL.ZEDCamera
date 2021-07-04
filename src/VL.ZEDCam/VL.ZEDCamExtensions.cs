using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Graphics;

namespace sl
{
    static public class ZEDCamExtensions
    {
        public static void GetTextureParameter(this Mat mat, out int width, out int height, out MipMapCount mitmap, out PixelFormat format, out TextureFlags textureFlags, out int arraySize, out GraphicsResourceUsage usage, out TextureOptions options, out DataPointer dataPointer)
        {
            width =  mat != null ? mat.GetWidth() : 256;
            height = mat != null ? mat.GetHeight() : 256;
            mitmap = 1;
            format = mat != null ? mat.GetMatType().GetPixelFormat() : PixelFormat.R32G32B32A32_Float;
            textureFlags = TextureFlags.ShaderResource;
            arraySize = 1;
            usage = GraphicsResourceUsage.Default;
            options = TextureOptions.None;
            dataPointer = mat != null ? new DataPointer(mat.GetPtr(), mat.GetWidth() * mat.GetHeight() * mat.GetPixelBytes()) : new DataPointer();
        }

        public static DataPointer  GetDataPointer(this Mat mat)
        { 
            return mat != null ? new DataPointer(mat.GetPtr(), mat.GetWidth() * mat.GetHeight() * mat.GetPixelBytes()) : new DataPointer();
        }

        public static void GetTextureParameter(this ZEDCam zEDCam, MEASURE measure, out int width, out int height, out MipMapCount mitmap, out PixelFormat format, out TextureFlags textureFlags, out int arraySize, out GraphicsResourceUsage usage, out TextureOptions options)
        {
            if (zEDCam != null)
            {
                zEDCam.GetResolution(out int w, out int h);

                if (w > 0 && h > 0)
                {
                    width = w;
                    height = h;
                }
                else
                {
                    width = 256;
                    height = 256;
                }
            }
            else
            {
                width = 256;
                height = 256;
            }
            mitmap = 1;
            format = measure.GetMatType().GetPixelFormat();
            textureFlags = TextureFlags.ShaderResource;
            arraySize = 1;
            usage = GraphicsResourceUsage.Default;
            options = TextureOptions.None;
        }

        public static void GetTextureParameter(this ZEDCam zEDCam, VIEW view, out int width, out int height, out MipMapCount mitmap, out PixelFormat format, out TextureFlags textureFlags, out int arraySize, out GraphicsResourceUsage usage, out TextureOptions options)
        {
            if (zEDCam != null)
            {
                zEDCam.GetResolution(out int w, out int h);

                if (w > 0 && h > 0)
                {
                    width = view == VIEW.SIDE_BY_SIDE ? width = 2 * w : w;
                    height = h;
                }
                else
                {
                    width = 256;
                    height = 256;
                }
            }
            else
            {
                width = 256;
                height = 256;
            } 
            mitmap = 1;
            format = view.GetMatType().GetPixelFormat();
            textureFlags = TextureFlags.ShaderResource;
            arraySize = 1;
            usage = GraphicsResourceUsage.Default;
            options = TextureOptions.None;
        }


        public static MAT_TYPE GetMatType(this VIEW view)
        {
            switch (view)
            {
                case VIEW.LEFT: return MAT_TYPE.MAT_8U_C4;
                case VIEW.RIGHT: return MAT_TYPE.MAT_8U_C4;
                case VIEW.LEFT_GREY: return MAT_TYPE.MAT_8U_C1;
                case VIEW.RIGHT_GREY: return MAT_TYPE.MAT_8U_C1;
                case VIEW.LEFT_UNRECTIFIED: return MAT_TYPE.MAT_8U_C4;
                case VIEW.RIGHT_UNRECTIFIED: return MAT_TYPE.MAT_8U_C4;
                case VIEW.LEFT_UNRECTIFIED_GREY: return MAT_TYPE.MAT_8U_C1;
                case VIEW.RIGHT_UNRECTIFIED_GREY: return MAT_TYPE.MAT_8U_C1;
                case VIEW.SIDE_BY_SIDE: return MAT_TYPE.MAT_8U_C4;
                case VIEW.DEPTH: return MAT_TYPE.MAT_8U_C4;
                case VIEW.CONFIDENCE: return MAT_TYPE.MAT_8U_C4;
                case VIEW.NORMALS: return MAT_TYPE.MAT_8U_C4;
                case VIEW.DEPTH_RIGHT: return MAT_TYPE.MAT_8U_C4;
                case VIEW.NORMALS_RIGHT: return MAT_TYPE.MAT_8U_C4;
                case VIEW.DEPTH_U16_MM: return MAT_TYPE.MAT_16U_C1;
                case VIEW.DEPTH_U16_MM_RIGHT: return MAT_TYPE.MAT_16U_C1;
                default: return MAT_TYPE.MAT_8U_C4;
            }
        }
        public static MAT_TYPE GetMatType(this MEASURE measure)
        {
            switch (measure)
            {
                case MEASURE.DISPARITY:return MAT_TYPE.MAT_32F_C1;
                case MEASURE.DEPTH:return MAT_TYPE.MAT_32F_C1;
                case MEASURE.CONFIDENCE:return MAT_TYPE.MAT_32F_C1;
                case MEASURE.XYZ:return MAT_TYPE.MAT_32F_C4;
                case MEASURE.XYZRGBA:return MAT_TYPE.MAT_32F_C4;
                case MEASURE.XYZBGRA:return MAT_TYPE.MAT_32F_C4;
                case MEASURE.XYZARGB:return MAT_TYPE.MAT_32F_C4;
                case MEASURE.XYZABGR:return MAT_TYPE.MAT_32F_C4;
                case MEASURE.NORMALS:return MAT_TYPE.MAT_32F_C4;
                case MEASURE.DISPARITY_RIGHT:return MAT_TYPE.MAT_32F_C1;
                case MEASURE.DEPTH_RIGHT:return MAT_TYPE.MAT_32F_C1;
                case MEASURE.XYZ_RIGHT:return MAT_TYPE.MAT_32F_C4;
                case MEASURE.XYZRGBA_RIGHT:return MAT_TYPE.MAT_32F_C4;
                case MEASURE.XYZBGRA_RIGHT:return MAT_TYPE.MAT_32F_C4;
                case MEASURE.XYZARGB_RIGHT:return MAT_TYPE.MAT_32F_C4;
                case MEASURE.XYZABGR_RIGHT:return MAT_TYPE.MAT_32F_C4;
                case MEASURE.NORMALS_RIGHT:return MAT_TYPE.MAT_32F_C4;
                default: return MAT_TYPE.MAT_32F_C4;
            }
        }
        public static PixelFormat GetPixelFormat(this MAT_TYPE matType)
        {
            switch (matType)
            {
                case MAT_TYPE.MAT_32F_C1: return PixelFormat.R32_Float;
                case MAT_TYPE.MAT_32F_C2: return PixelFormat.R32G32_Float;
                case MAT_TYPE.MAT_32F_C3: return PixelFormat.R32G32B32_Float;
                case MAT_TYPE.MAT_32F_C4: return PixelFormat.R32G32B32A32_Float;
                case MAT_TYPE.MAT_8U_C1: return PixelFormat.A8_UNorm;
                case MAT_TYPE.MAT_8U_C2: return PixelFormat.R8G8_UNorm;
                case MAT_TYPE.MAT_8U_C3: return PixelFormat.B8G8R8A8_UNorm; // TODO DONT EXIST
                case MAT_TYPE.MAT_8U_C4: return PixelFormat.B8G8R8A8_UNorm;
                case MAT_TYPE.MAT_16U_C1: return PixelFormat.R16_UInt; ;
                default: return PixelFormat.R32G32B32A32_Float;
            }
        }

        public static (int channel, int bytes) GetChannelAndByts(this MAT_TYPE matType)
        {
            switch (matType)
            {
                case MAT_TYPE.MAT_32F_C1: return (1,4);
                case MAT_TYPE.MAT_32F_C2: return (2, 8);
                case MAT_TYPE.MAT_32F_C3: return (3, 12);
                case MAT_TYPE.MAT_32F_C4: return (4, 16);
                case MAT_TYPE.MAT_8U_C1: return (1, 1);
                case MAT_TYPE.MAT_8U_C2: return (2, 2);
                case MAT_TYPE.MAT_8U_C3: return (3, 3); 
                case MAT_TYPE.MAT_8U_C4: return (4, 4);
                case MAT_TYPE.MAT_16U_C1: return (1, 2);
                default: return (4, 16);
            }
        }
        public static (int channel, int bytes) GetChannelAndByts(this Mat mat)
        {
            return (mat.GetChannels(), mat.GetPixelBytes());
        }
        public static MAT_TYPE GetMatType(this Mat mat)
        {
            switch (mat.GetChannelAndByts())
            {
                case (1, 4): return MAT_TYPE.MAT_32F_C1;
                case (2, 8): return MAT_TYPE.MAT_32F_C2;
                case (3, 12): return MAT_TYPE.MAT_32F_C3;
                case (4, 16): return MAT_TYPE.MAT_32F_C4;
                case (1, 1): return MAT_TYPE.MAT_8U_C1;
                case (2, 2): return MAT_TYPE.MAT_8U_C2;
                case (3, 3): return MAT_TYPE.MAT_8U_C3;
                case (4, 4): return MAT_TYPE.MAT_8U_C4;
                case (1, 2): return MAT_TYPE.MAT_16U_C1;
                default: return MAT_TYPE.MAT_32F_C4;
            }
        }
    }
}
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        