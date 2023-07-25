using UnityEngine;
using UnityEngine.InputSystem;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        // [SerializeField]private Transform SteeringWheel = null;
        float distanceTravelled;
        private InputMaster controls;
        private float direction;
        private bool isStop = false;

        // private GameManager gm;
        // [SerializeField]private AudioClip ac_carHorn;
        [SerializeField]private AudioClip ac_carIgnition;
        [SerializeField]private AudioClip ac_carEngine;
        

        void OnEnable() {
            controls.Enable();
            controls.Player.Move.started += MoveCar;
            controls.Player.Move.performed += MoveCar;
            controls.Player.Move.canceled += MoveCar;

            controls.Player.Honk.started += _ => Honk();
            controls.Player.Honk.performed += _ => Honk();
            controls.Player.Honk.canceled += _ => Honk();
        }

        void OnDisable() {
            controls.Disable();
            controls.Player.Move.started -= MoveCar;
            controls.Player.Move.performed -= MoveCar;
            controls.Player.Move.canceled -= MoveCar;

            controls.Player.Honk.started -= _ => Honk();
            controls.Player.Honk.performed -= _ => Honk();
            controls.Player.Honk.canceled -= _ => Honk();
        }

        private void Awake() {
            controls = new InputMaster(); 
            // gm = GameManager.Instance; // breaks gameplay, dont cache it
        }

        void Start() {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
                distanceTravelled +=  1 * 5 * Time.deltaTime;
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction) + new Vector3(0,0.5f,0);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
            }
        }

        void Update()
        {
            if (pathCreator != null)
            {
                if(GameManager.Instance.canDrive){
                    distanceTravelled +=  direction * speed * Time.deltaTime;
                    transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction) + new Vector3(0,0.5f,0);
                    transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                    Debug.Log("distanceTravelled: "+distanceTravelled);
                }
                
                if(!isStop){
                    if((distanceTravelled > 66) && (distanceTravelled < 66.1)&& (direction != 0)){
                        Debug.LogWarning("STOPPPP!!!!!!!");
                        GameManager.Instance.setDistanceTravelled(distanceTravelled);
                        isStop = true;
                        distanceTravelled +=  1 * 5 * Time.deltaTime;
                        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction) + new Vector3(0,0.5f,0);
                        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                        // controls.Enable();
                        // controls.Player.Move.started += MoveCar;
                        // controls.Player.Move.performed += MoveCar;
                        // controls.Player.Move.canceled += MoveCar;
                    }
                }

                if((distanceTravelled > 139) && (distanceTravelled < 140)){
                    GameManager.Instance.setDistanceTravelled(distanceTravelled);
                }
                
            }
        }

        void MoveCar(InputAction.CallbackContext context){
            direction = context.ReadValue<float>();
        }

        private void Honk(){
            GetComponent<AudioSource>().Play();
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}