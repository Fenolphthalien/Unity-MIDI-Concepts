using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class FloorBlock : MonoBehaviour
{
	int m_health = kMaxHealth;

	Renderer m_renderer;

	bool m_broke;

	public bool isWalkable{get{return m_health > 0 && !m_broke;}}

	const int kMaxHealth = 100;
	const float kRepairAfter = 2f, kRepairTime = 2f;

	Coroutine m_activeCoroutine;

	void Awake()
	{
		m_renderer = GetComponent<Renderer>();
	}

	public void Initialise()
	{
		m_health = kMaxHealth;
		if (m_activeCoroutine != null)
			StopCoroutine (m_activeCoroutine);
		m_broke = false;
		SetSpriteColour (Color.white);
	}

	public void ApplyDamage(int amount)
	{
		if (!m_broke) 
		{
			m_health -= amount;
			m_health = !isWalkable ? 0 : m_health; // Move back to zero if less than zero.
				
			SetSpriteColour(new Color (1, m_health * 0.01f, m_health * 0.01f, 1));
			if(!isWalkable)
			{
				BlockBroken();
			}
		}
	}

	void BlockBroken()
	{
		m_broke = true;
		SetSpriteColour (new Color (1, 1, 1, 0));
		m_activeCoroutine = StartCoroutine (Repair());
	}

	public void ApplyDamage(int amount, int arrayIndex)
	{
		ApplyDamage (amount);
	}

	void SetSpriteColour(Color c)
	{
		SpriteRenderer spriteRenderer = (SpriteRenderer)m_renderer;
		if (spriteRenderer != null)
			spriteRenderer.color = c;
	}

	IEnumerator Repair()
	{
		const float repairDiv = 100f / (kRepairTime != 0 ? kRepairTime : 1f) ;

		float tempHealth = m_health;

		yield return new WaitForSeconds (kRepairAfter);
		while(m_health < 100)
		{
			tempHealth += Time.deltaTime * repairDiv;
			m_health = (int)tempHealth;
			SetSpriteColour (new Color (1, 1, 1, tempHealth * 0.01f));
			yield return new WaitForEndOfFrame();
		}
		SetSpriteColour ( new Color (1, 1, 1, 1));
		m_broke = false;
	}

	public int GetHealth()
	{
		return m_broke ? 0 : m_health;
	}

}
