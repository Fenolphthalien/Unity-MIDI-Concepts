using UnityEngine;
using System.Collections;

public interface ICollide
{
	void PerformCollision(ICollide collide);
}
public interface IContainObjects
{
	void Clear();
}
