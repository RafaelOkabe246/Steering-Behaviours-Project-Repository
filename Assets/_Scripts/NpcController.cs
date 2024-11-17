using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NpcState
{
    Pegador,
    Fugitivo
}

public enum NpcActions
{
    Wander,
    Hide,
    HideInteligent,
    Flee,
    Evade,
    Persue
}

public class NpcController : MonoBehaviour
{
    [SerializeField] private NpcActions npcActions;
    [SerializeField] private NpcState npcState;
    public LayerMask npcLayer;

    public MeshRenderer mesh;

    public float currentSpeed;

    [Header("Game stats")]
    public bool isPlaying;
    public Color winnerColor = Color.yellow;
    public Color notPlayColor = Color.gray;

    [Header("Pegador stats")]
    public float pegadorSpeed = 5f;
    public float searchDistance = 8f;
    [SerializeField] private Color pegadorColor = Color.red;

    [Header("Fugitivo stats")]
    public float fugitivoSpeed = 3f;
    public float evadeDistance = 10f;
    [SerializeField] private Color fugitivoColor = Color.green;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;


    private GameObject helper;
    private PlayerController targetController;


    Vector3 wanderTarget = Vector3.zero;
    private GameObject point;
    [SerializeField] bool coolDown = false;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //target = GameObject.FindGameObjectWithTag("Player").transform;
        //targetController = target.GetComponent<PlayerController>();
        
        helper = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        DestroyImmediate(helper.GetComponent<Collider>());
        helper.GetComponent<MeshRenderer>().material.color = Color.red;
        helper.SetActive(false);

        point = GameObject.CreatePrimitive(PrimitiveType.Sphere).gameObject;
        DestroyImmediate(point.GetComponent<Collider>());
        point.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        point.GetComponent<MeshRenderer>().material.color = Color.green;
        point.gameObject.SetActive(false);
    }


    private void Update()
    {
        if (!isPlaying)
            return;

        switch (npcState)
        {
            case NpcState.Pegador:
                PegadorBehaviour();
                break;
            case NpcState.Fugitivo:
                FugitivoBehaviour();
                break;
        }

        //Professor code
        //if (!coolDown)
        //{
        //    if (!TargetInRange()) 
        //        Wander();
        //    else
        //    {
        //        if (CanSeeTarget() && CanSeeMe())
        //        {
        //            coolDown = true;
        //            HideInteligent();
        //            Invoke("CoolDown", 5f);
        //        }
        //        else 
        //            Pursue();
        //    }
        //}
    }

    private void OnCollisionEnter(Collision other)
    {
        if (npcState == NpcState.Pegador || coolDown)
            return;

        if (other.gameObject.TryGetComponent<NpcController>(out NpcController npcController))
        {
            if (npcController == NpcManager.currentPegador)
            {
                Debug.Log("Detected noc");

                NpcManager.instance.ChangePegador(this);
            }
        }
    }

    #region Pega pega methods

    public void SetIsPlaying(bool value)
    {
        isPlaying = value;
        if (!value)
            mesh.materials[0].color = notPlayColor;
    }

    public void SetWinnerColor()
    {
        mesh.materials[0].color = winnerColor;

    }

    void PegadorBehaviour()
    {
        searchDistance = 15f;

        SearchTargets();


         if(!coolDown)
        {
            Pursue();
        }
        else
        {
            Wander();
        }

    }

    void FugitivoBehaviour()
    {
        target = NpcManager.currentPegador.transform;
        Evade();

        if (!TargetInRange())
        {
            Wander();
        }
        else if(TargetInRange() && CanSeeMe())
        {
            if(!coolDown)
            {
                int i = Random.Range(0, 2);
                if(i == 0)
                {
                    Evade();
                }
                else
                {
                    Flee(target.position);
                }
                coolDown = true;
                Invoke("CoolDown", 3f);
            }
        }
        
    }

    public void ChangeState(NpcState newNpcState)
    {

        npcState = newNpcState;
        coolDown = true;
        Invoke("CoolDown", 3f);
        switch (npcState)
        {
            case NpcState.Pegador:
                agent.speed = pegadorSpeed;

                mesh.materials[0].color = pegadorColor;
                break;
            case NpcState.Fugitivo:
                agent.speed = fugitivoSpeed;

                mesh.materials[0].color = fugitivoColor ;

                break;
        }
    }

    #endregion

    #region Professor codes
    void Seek(Vector3 targetPos)
    {
        agent.SetDestination(targetPos);
        //helper.SetActive(true);
        if (helper.activeSelf == true)
        {
            helper.transform.position = new Vector3(targetPos.x, 0, targetPos.z);
        }
    }


    //Fuga
    void Flee(Vector3 targetPos)
    {
        npcActions = NpcActions.Flee;
        Vector3 vetorFuga = transform.position - targetPos;
        agent.SetDestination(transform.position + vetorFuga);
        //helper.SetActive(true);
        if (helper.activeSelf == true)
        {
            helper.transform.position = transform.position + vetorFuga;
        }
    }
    //Passeio
    void Wander()
    {
        npcActions = NpcActions.Wander;
        float wanderRadius = 10f;
        float wanderDistance = 10f;
        float wanderJitter = 1f;

        // gera um vetor aleatório em x e z
        float randomX = Random.Range(-1f, 1f) * wanderJitter;
        float randomZ = Random.Range(-1f, 1f) * wanderJitter;
        wanderTarget += new Vector3(randomX, 0, randomZ);

        // normaliza o vetor e multiplica pelo raio do círculo de passeio
        // para que o alvo fique na borda do círculo
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        // adiciona a distância de passeio ao vetorza o vetor e multiplica pelo raio do círculo de passeio
        // para que o alvo fique na borda do círculo
        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        
        // transforma o vetor do espaço local para o espaço do mundo        
        Vector3 targetWorld = transform.TransformVector(targetLocal);
        Seek(targetWorld);

    }

    void Evade() 
    {
        npcActions = NpcActions.Evade;
        Vector3 targetDir = transform.position - target.transform.position;
        float speeds = agent.speed + target.GetComponent<NavMeshAgent>().speed;//targetController.actualSpeed;
        float lookAhead = (targetDir.magnitude / speeds) + evadeDistance;
        Vector3 targetPred = target.transform.position + target.transform.forward * lookAhead;

        Seek(targetPred);
    }


    void Pursue() 
    {
        npcActions = NpcActions.Persue;
        Vector3 targetDir = target.transform.position - transform.position;

        // obtém o ângulo entre a direção do agente e a direção do alvo, transformada para o espaço local do agente        
        float relativeHeading = Vector3.Angle(transform.forward, transform.InverseTransformVector(targetDir));

        // obtém o ângulo entre a direção frontal do agente e a direção ao alvo (no espaço local do agente)
        float angleToTarget = Vector3.Angle(transform.forward, transform.TransformVector(targetDir));

        NavMeshAgent targetAgent = target.GetComponent<NavMeshAgent>();

        // Se o alvo estiver fora do campo de visão direto do agente (ângulo maior que 90) mas orientado quase na mesma direção (diferença de direção menor que 20), ou se o alvo estiver quase parado, o agente busca diretamente a posição do alvo."
        if((angleToTarget > 90 && relativeHeading < 20 || targetAgent.speed < 0.01f))//targetController.actualSpeed < 0.01f))
        {
            Seek(target.position); 
            return;
        }

        float speeds = agent.speed + targetAgent.speed;//targetController.actualSpeed;
        float lookAhead = targetDir.magnitude / speeds;

        Vector3 targetPred = target.transform.position + target.transform.forward * lookAhead;
        Seek(targetPred);
    }


    //Esconder
    void Hide()
    {
        float distance = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;
        for (int i = 0; i < GameEnvironment.GetInstance.GetHidingSpots().Length; i++)
        {
            // calcula a distância do alvo até o esconderijo
            Vector3 hideDir = GameEnvironment.GetInstance.GetHidingSpots()[i].transform.position - target.transform.position;
            Vector3 hidePos = GameEnvironment.GetInstance.GetHidingSpots()[i].transform.position + hideDir.normalized * 5;
            if (Vector3.Distance(transform.position, hidePos) < distance)
            {
                chosenSpot = hidePos;
                distance = Vector3.Distance(transform.position, hidePos);
            }
        }
        Seek(chosenSpot);
        npcActions = NpcActions.Hide;
    }

    //Esconder inteligente
    void HideInteligent()
    {

        float distance = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;
        Vector3 chosenDirection = Vector3.zero;
        GameObject chosenGO = null;
        for (int i = 0; i < GameEnvironment.GetInstance.GetHidingSpots().Length; i++)
        {
            // calcula a distância do alvo até o esconderijo
            Vector3 hideDir = GameEnvironment.GetInstance.GetHidingSpots()[i].transform.position - target.transform.position;
            Vector3 hidePos = GameEnvironment.GetInstance.GetHidingSpots()[i].transform.position + hideDir.normalized * 10;
            if (Vector3.Distance(transform.position, hidePos) < distance)
            {
                chosenSpot = hidePos;
                chosenDirection = hideDir;
                chosenGO = GameEnvironment.GetInstance.GetHidingSpots()[i];
                distance = Vector3.Distance(transform.position, hidePos);
            }
        }

        Collider hideCollider = chosenGO.GetComponent<Collider>();
        chosenSpot.y = 0.5f;
        Vector3 cDirNorm = new Vector3(chosenDirection.x, 0, chosenDirection.z).normalized;
        Ray backRay = new Ray(chosenSpot, -cDirNorm);
        RaycastHit hit;
        hideCollider.Raycast(backRay, out hit, 100.0f);
        Seek(hit.point + cDirNorm * 2.0f);
        // Debug.DrawRay(chosenSpot, -cDirNorm * 10, Color.red);

        //point.SetActive(true);
        if (point.activeSelf == true)
        {
            point.transform.position = hit.point;
        }

        npcActions = NpcActions.HideInteligent;
    }

    #endregion

    #region Detection_methods

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, searchDistance) ;
    }
    Transform newTarget;

    void SearchTargets()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, searchDistance, Vector3.right, 0, npcLayer, QueryTriggerInteraction.UseGlobal);
        float distance = Mathf.Infinity;
        if (hits.Length == 0)
            return;

        foreach (RaycastHit hit in hits)
        {
            float newCheckDistance = Vector3.Distance(transform.position, hit.collider.gameObject.transform.position);
            if (newCheckDistance < distance && hit.collider.gameObject != this.gameObject)
            {
                distance = newCheckDistance;
                newTarget = hit.transform;
            }

        }


        //for (int i = 0; i < hits.Length; i++)
        //{
        //    float newCheckDistance = Vector3.Distance(transform.position, hits[i].collider.gameObject.transform.position);
        //    if ( newCheckDistance < distance && hits[i].collider.gameObject.layer == npcLayer
        //        && hits[i].collider.gameObject != this.gameObject)
        //    {
        //        distance = newCheckDistance;
        //        newTarget = hits[i].transform;
        //    }
        //}

        if (newTarget != this.transform && newTarget != null)
        {
            target = newTarget;

        }
    }

    bool CanSeeTarget()
    {
        RaycastHit hit;
        Vector3 rayToTarget = target.position - transform.position;
        float lookAngle = Vector3.Angle(rayToTarget, transform.forward);
        if (lookAngle < 60 && Physics.Raycast(transform.position, rayToTarget, out hit))
        {
            if (hit.collider.gameObject.transform == target)
            {
                return true;
            }
        }
        return false;
    }

    bool CanSeeMe()
    {
        Vector3 rayToTarget = transform.position - target.transform.position;
        float lookAngle = Vector3.Angle(rayToTarget, target.transform.forward);
        if (lookAngle < 60)
        {
            return true;
        }
        return false;
    }

    void CoolDown() { coolDown = false; }
    bool TargetInRange()
    {
        Vector3 distance = target.position - transform.position;
        if (distance.magnitude < 10)
        {
            return true;
        }
        return false;
    }

    #endregion

}
