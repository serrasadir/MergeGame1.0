using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ProducerAnimation : MonoBehaviour
{
    public GameObject circlePrefab;
    public GameObject sparklePrefab;
    public GameObject particlePrefab;
    public int numberOfParticles = 10;
    public float explosionDistance = 100f;
    public float upForce = 1f;
    public float particleLifetime = 100f;


    public void GenerateProducer(Vector3 position)
    {
        GameObject circleInstance = Instantiate(circlePrefab, transform);
        circleInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);

        circleInstance.transform.position = position;
        circleInstance.transform.localScale = Vector3.zero;


        circleInstance.transform.DOScale(2.0f, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            circleInstance.transform.DOScale(0, 0.7f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                Destroy(circleInstance);
            });
        });
    }


    public void DestroyProducer(Vector3 position)
    {
        for (int i = 0; i < numberOfParticles; i++)
        {
            GameObject particle = Instantiate(particlePrefab, transform);
            particle.transform.SetParent(GameObject.Find("Canvas").transform, false);

            particle.transform.position = position;

            RectTransform rect = particle.GetComponent<RectTransform>();

            Vector2 spreadDirection = Random.insideUnitCircle.normalized;
            rect.DOAnchorPos(rect.anchoredPosition + spreadDirection * 500f, 20f)
                .SetEase(Ease.OutQuad);

            Image img = particle.GetComponent<Image>();
            img.DOFade(0, particleLifetime).OnComplete(() => Destroy(particle));
        }
        Image producerImage = GetComponent<Image>();
        producerImage.DOFade(0, 5.5f);
        
        Destroy(gameObject, 5.5f); 
    }


    public void MergeAppliances(Vector3 position)
    {
        float fastLifetime = 3.0f; 
        float fastExplosionDistance = 300f; 

        for (int i = 0; i < numberOfParticles; i++)
        {
            GameObject particle = Instantiate(sparklePrefab, transform);
            particle.transform.SetParent(GameObject.Find("Canvas").transform, false);

            RectTransform rect = particle.GetComponent<RectTransform>();

            particle.transform.position = position;

            Vector2 spreadDirection = Random.insideUnitCircle.normalized;
            rect.DOAnchorPos(rect.anchoredPosition + spreadDirection * fastExplosionDistance, fastLifetime)
                .SetEase(Ease.OutQuad);

            Image img = particle.GetComponent<Image>();
            img.DOFade(0, fastLifetime).OnComplete(() => Destroy(particle));
        }
        Image producerImage = GetComponent<Image>();
        producerImage.DOFade(0, 0.5f);
    }

}


