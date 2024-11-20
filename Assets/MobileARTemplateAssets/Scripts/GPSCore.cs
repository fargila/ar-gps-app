using System.Collections;
using UnityEngine;

public class GPSCore : MonoBehaviour
{
    // Array para armazenar as latitudes dos pontos de interesse, no caso serão 3 pontos no projeto: Auditório, EVA, Complexo de Tecnologia.
    public double[] Lat;
    // Array para armazenar as longitudes dos pontos de interesse.
    public double[] Lon;
    // Contador para acompanhar qual ponto estamos no array.
    internal int pointCounter = 0;
    // Variável para armazenar a distância calculada entre dois pontos.
    private double distance;
    // Posição original do objeto, não utilizado no código atual.
    private Vector3 originalPosition;
    // Raio de alcance em metros para ativar o pop-up.
    public float radius = 1f;
    // Intervalo de atualização da localização em segundos.
    public float update = 3f;
    // Array de GameObjects que representam os pop-ups de cada ponto.
    public GameObject[] targetPopUp;
    // Flag para garantir que o pop-up seja ativado apenas uma vez.
    public bool targetPopUpOneTime = false;

    // Variáveis para armazenar a latitude e longitude atuais do dispositivo.
    private double lat;
    private double lon;

    private void Start()
    {
        // Inicia o serviço de localização do dispositivo.
        Input.location.Start();
        // Inicia a coroutine que processa as informações de GPS.
        StartCoroutine(GPSProcess());
    }

    // Coroutine que atualiza a posição a cada intervalo definido.
    public IEnumerator GPSProcess()
    {
        while (true)
        {
            // Aguarda o intervalo de atualização.
            yield return new WaitForSeconds(update);

            // Verifica se o serviço de localização está habilitado pelo usuário.
            if (Input.location.isEnabledByUser)
            {
                // Obtém a latitude e longitude atuais.
                lat = Input.location.lastData.latitude;
                lon = Input.location.lastData.longitude;

                // Verifica se ainda há pontos a serem processados.
                if (pointCounter < Lat.Length && pointCounter < Lon.Length)
                {
                    // Chama a função de cálculo de distância.
                    Calc(Lat[pointCounter], Lon[pointCounter], lat, lon);
                }
            }
        }
    }

    // Calcula a distância entre dois pontos geográficos usando a fórmula de Haversine.
    public void Calc(double lat1, double lon1, double lat2, double lon2)
    {
        // Raio da Terra em metros.
        var R = 6378137;

        var dLat = (lat2 - lat1) * Mathf.PI / 180;
        var dLon = (lon2 - lon1) * Mathf.PI / 180;

        
        var a = Mathf.Sin((float)dLat / 2) * Mathf.Sin((float)dLat / 2) +
                Mathf.Cos((float)(lat1 * Mathf.PI / 180)) * Mathf.Cos((float)(lat2 * Mathf.PI / 180)) *
                Mathf.Sin((float)dLon / 2) * Mathf.Sin((float)dLon / 2);
        var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        
        distance = R * c;

        // Verifica se a distância é menor que o raio especificado.
        if (distance < radius)
        {
            // Ativa o pop-up do ponto se ainda não tiver sido ativado.
            if (!targetPopUpOneTime)
            {
                for(int i = 0; i < targetPopUp.Length; i++)
                {
                    targetPopUp[i].SetActive(false); // Desativa todos os pop-ups.
                }
                targetPopUp[pointCounter].SetActive(true); // Ativa o pop-up do ponto atual.
                targetPopUpOneTime = true; // Marca que o pop-up foi ativado.
            }
        }
        else
        {
            // Se a distância é maior que o raio, desativa o pop-up atual.
            for(int i = 0; i < targetPopUp.Length; i++)
            {
                targetPopUp[i].SetActive(false); // Desativa todos os pop-ups.
            }
            targetPopUp[pointCounter].SetActive(false); // Desativa o pop-up do ponto atual.
            targetPopUpOneTime = false; // Permite que o pop-up seja ativado novamente.

            // Move para o próximo ponto.
            pointCounter++;

            // Se todos os pontos foram processados, reinicia o contador.
            if (pointCounter >= Lat.Length || pointCounter >= Lon.Length)
            {
                pointCounter = 0;
            }
        }
    }

    // Método para ocultar o pop-up do ponto atual.
    public void HideTargetPopUp()
    {
        targetPopUp[pointCounter].SetActive(false); // Desativa o pop-up do ponto atual.
        targetPopUpOneTime = true; // Marca que o pop-up foi ocultado.
    }

    // void Update()
    // {
    // }
}
