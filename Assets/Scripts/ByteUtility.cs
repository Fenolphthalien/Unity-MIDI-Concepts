using UnityEngine;
using System;
using System.Collections;

/*
#define MAKEWORD(a, b)      ((WORD)(((BYTE)(((DWORD_PTR)(a)) & 0xff)) | ((WORD)((BYTE)(((DWORD_PTR)(b)) & 0xff))) << 8))
#define MAKELONG(a, b)      ((LONG)(((WORD)(((DWORD_PTR)(a)) & 0xffff)) | ((DWORD)((WORD)(((DWORD_PTR)(b)) & 0xffff))) << 16))
#define LOWORD(l)           ((WORD)(((DWORD_PTR)(l)) & 0xffff))
#define HIWORD(l)           ((WORD)((((DWORD_PTR)(l)) >> 16) & 0xffff))
#define LOBYTE(w)           ((BYTE)(((DWORD_PTR)(w)) & 0xff))
#define HIBYTE(w)           ((BYTE)((((DWORD_PTR)(w)) >> 8) & 0xff))
*/


public static class TypeUtility
{

	public static UInt16 LoWord(uint dword)
	{
		return (UInt16)(dword & 0xfffff);
	}

	public static UInt16 HiWord(uint dword)
	{
		return (UInt16)((dword >> 16) & 0xffff);
	}

	public static byte LoByte(UInt16 word)
	{
		return (byte)(word & 0xff);
	}

	public static byte HiByte(UInt16 word)
	{
		return (byte)((word >> 8) & 0xff);
	}

	public static void SplitDoubleWord (uint dword, out UInt16 hi, out UInt16 lo){
		hi = HiWord (dword);
		lo = LoWord (dword);
	}

	public static void SplitWord(UInt16 word, out byte hi, out byte lo)
	{
		hi = HiByte (word);
		lo = LoByte (word);
	}
}
