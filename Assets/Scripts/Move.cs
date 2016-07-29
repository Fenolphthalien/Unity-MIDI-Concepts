using UnityEngine;
using System.Collections;

namespace Tween
{
	public static class Move
	{
		public static bool MoveTo(ref float A, float B)
		{
			float Distance = B - A;
			if(Mathf.Abs(Distance) >= Time.deltaTime)
			{
				A += Time.deltaTime * Mathf.Sign(Distance);
			}
			else
			{
				A += Distance;
			}
			return  A == B;
		}

		public static bool MoveTo(ref float A, float B, byte Speed)
		{
			float Distance = B - A;
			if(Mathf.Abs(Distance) >= (Time.deltaTime * Speed))
			{
				A += Time.deltaTime * Mathf.Sign(Distance) * Speed;
			}
			else
			{
				A += Distance;
			}
			return  A == B;
		}

		public static bool MoveTo(ref short A, short B)
		{
			short Distance = (short)(B - A);
			if(Distance >= Time.deltaTime)
			{
				A += (short)Mathf.CeilToInt(Time.deltaTime);
			}
			else
			{
				A += Distance;
			}
			return  A == B;
		}

		public static bool MoveTo(ref short A, short B, byte Speed)
		{
			short Distance = (short)(B - A);
			if(Distance >= (Time.deltaTime * Speed))
			{
				A += (short)(Time.deltaTime * Speed);
			}
			else
			{
				A += Distance;
			}
			return  A == B;
		}

		public static bool MoveTo(Transform A, Vector3 Target)
		{
			Vector3 Distance = (Target - A.position);
			if(Distance.magnitude >= Time.deltaTime)
			{
				A.localPosition += Distance.normalized * Time.deltaTime;
			}
			else
			{
				A.localPosition += Distance;
			}
			return  A.localPosition == Target;
		}

		public static bool MoveTo(Transform A, Vector3 Target, byte Speed)
		{
			Vector3 Distance = (Target - A.position);
			if(Distance.magnitude >= Time.deltaTime * Speed)
			{
				A.localPosition += Distance.normalized * (Time.deltaTime * Speed);
			}
			else
			{
				A.localPosition += Distance;
			}
			return  A.localPosition == Target;
		}

		public static bool MoveTo(RectTransform A, Vector3 Target)
		{
			Vector3 Distance = (Target - A.localPosition);
			if(Distance.magnitude >= Time.deltaTime)
			{
				A.localPosition += Distance.normalized * Time.deltaTime;
			}
			else
			{
				A.localPosition += Distance;
			}
			return  A.localPosition == Target;
		}

		public static bool MoveTo(RectTransform A, Vector3 Target, int speed)
		{
			Vector3 Distance = (Target - A.localPosition);
			if(Distance.magnitude >= Time.deltaTime * speed)
			{
				A.localPosition += Distance.normalized * (Time.deltaTime * speed);
			}
			else
			{
				A.localPosition += Distance;
			}
			return  A.localPosition == Target;
		}

		public static bool MoveTo(ref Vector2 A, Vector2 B)
		{
			Vector2 Distance = B - A;
			if(Distance.magnitude >= Time.deltaTime)
			{
				A += Distance.normalized * Time.deltaTime;
			}
			else
			{
				A += Distance;
			}
			return  A == B;
		}

		public static bool MoveTo(ref Vector3 A, Vector3 B)
		{
			Vector3 Distance = B - A;
			if(Distance.magnitude >= Time.deltaTime)
			{
				A += Distance.normalized * Time.deltaTime;
			}
			else
			{
				A += Distance;
			}
			return  A == B;
		}

		public static bool MoveTo(ref Color A, Color B)
		{
			if(A == B)
			{
				return true;
			}
			Vector4 Distance = new Vector4 (B.r - A.r, B.g - A.g, B.b - A.b, B.a - A.a);
			if(Distance.magnitude >= Time.deltaTime)
			{
				Distance.Normalize();
				A += new Color(Distance.x,Distance.y,Distance.z,Distance.w);
			}
			else
			{
				A += new Color(Distance.x,Distance.y,Distance.z,Distance.w);
			}
			return  A == B;
		}

		public static Color MoveTo(this Color A, Color B)
		{
			Vector4 Distance = new Vector4 (B.r - A.r, B.g - A.g, B.b - A.b, B.a - A.a);
			if(Distance.magnitude >= Time.deltaTime){
				Distance.Normalize();
				A += new Color(Distance.x,Distance.y,Distance.z,Distance.w) * Time.deltaTime;
			}
			else{
				A += new Color(Distance.x,Distance.y,Distance.z,Distance.w);
			}
			return  A;
		}

		public static Color FadeTo(this Color A, float B)
		{
			float Distance = B - A.a;
			if(Distance >= Time.deltaTime){
				A += new Color(0,0,0,1f) * Time.deltaTime;
			}
			else{
				A += new Color(0,0,0,Distance);
			}
			return  A;
		}

		public static bool MoveToLocal(Transform A, Vector3 Target, byte Speed)
		{
			Vector3 Distance = (Target - A.localPosition);
			if(Distance.magnitude >= Time.deltaTime * Speed)
			{
				A.localPosition += Distance.normalized * (Time.deltaTime * Speed);
			}
			else
			{
				A.localPosition += Distance;
			}
			return  A.localPosition == Target;
		}

		public static bool EaseToLocal(Transform A, Vector3 Target, byte Speed)
		{
			Vector3 Distance = (Target - A.localPosition);
			A.localPosition += Distance * (Time.deltaTime * Speed);
			return  Distance.magnitude <= 0;
		}

		public static bool EaseToLocal(Transform A, Vector3 Target, byte Speed, float Leeway)
		{
			Vector3 Distance = (Target - A.localPosition);
			A.localPosition += Distance * (Time.deltaTime * Speed);
			return  Distance.magnitude <= Leeway;
		}

		public static bool EaseTo(ref float A, float B)
		{
			float Distance = (B - A);
			A += Distance * Time.deltaTime;
			return  Distance <= 0;
		}

		public static bool EaseTo(ref float A, float B, float Speed, float Max)
		{
			float Distance = (B - A);
			A += Distance * Mathf.Min(Time.deltaTime * Speed, Max);
			return  Distance <= 0;
		}

		public static bool EaseTo(ref float A, float B, float Speed)
		{
			float Distance = (B - A);
			A += Distance * (Time.deltaTime * Speed);
			return  Distance <= 0;
		}

		public static bool EaseTo(ref short A, short B, byte Speed)
		{
			float Distance = (B - A);
			A += (short)Mathf.CeilToInt(Distance * (Time.deltaTime * Speed));
			return  Distance < 0;
		}

		public static void EaseTo(ref Vector3 A, Vector3 B, byte Speed)
		{
			Vector3 Distance = B - A;
			A += Distance * (Time.deltaTime * Speed);
		}
		
		public static Vector3 Align(Vector3 A)
		{
			return new Vector3 (A.x, A.y, 10);
		}
		
		public static Vector3 Align(float x, float y)
		{
			return new Vector3 (x, y, 10);
		}
	}
   
    public static class Functions
    {
        /* https://www.youtube.com/watch?v=3D0PeJh6GY8 */
        public static float EaseInOut(float x, float a)
        {
            return x * a / x * a + (1 - x) * a;
        }
    }
}
