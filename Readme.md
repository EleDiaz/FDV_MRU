# Moving forward on Unity

Let's move some objects around the scene. One of them will play catch and the other will be fully manage by the user. To achieve this will overuse a simple formula to represent movement of an object througth time.

```c-sharp
position = position_0 + velocity * time_lapse
```

- `position_0` it will be our initial position, it can be a 2d or 3d vector.
- `time_lapse` will be the diff between when `position_0` was calculated and the actual time. Also named delta time. Which in our case will be the time-lapse between each new frame generate by the render.

```c-sharp
    // CharacterMovement.cs
    void Update()
    {
        // We get the actual user input `movementAction` is an `InputActionReference`
        var direction = movementAction.action.ReadValue<Vector2>();
        if (direction != Vector2.zero)
        {
            Vector3 movement = new Vector3(direction.x, 0f, direction.y);
            // We apply the already mentioned formula.
            transform.Translate(movement * Time.deltaTime * speed, Space.Self);
        }

        var look = lookAround.action.ReadValue<Vector2>();
        if (look != Vector2.zero)
        {
            Vector3 rotation = new Vector3(0f,look.x, 0f);
            // We can alse use the formula to calculate rotation around the axis Y
            transform.Rotate(rotation * Time.deltaTime * rotationSpeed, Space.Self);
        }
    }
```

For the case of the object that will keep following the player. The direction won't be given by the user, but by the direction where the player is place in relation with this object.

```c-sharp
        // SimpleFollow.cs
        var direction = target.transform.position - transform.position;
        // ...
        // To later apply the translation. DON'T forget to normalized the direction.
        transform.Translate(direction.normalized * Time.deltaTime * speed, Space.World);
```

There ain't fun yet. We need some obstacles, some difficulty throught the gameplay and, an objective.

Let's generate some random objects around the scene.

```c-sharp
        // GenerateMap.cs
        for (int i = 0; i < amount; i++)
        {
            // I need some space for player to start, but with limits. This isn't No man's Sky.
            var distance = Random.Range(innerRadius, outerRadius);
            // The angle to where will be placing our obstacle
            var angle = Random.Range(0, 360f);
            // Quaternion magic, will rotate the vector from our initial position to certain distance.
            var position = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * distance + initialOrigin.position;

            // Building our world
            var gameObject = Instantiate(obstacle, position, Quaternion.identity);
            gameObject.name = "Obstacle " + i;
            // ...
        }
```

It isn't enough. They are just objects in random positions with no interaction. The player has to bring life to those.

```c-sharp
    // PlayerReaction.cs
    void OnCollisionEnter(Collision other)
    {
        // When we touch those objects with the player, those will react to us.
        if (other.gameObject.CompareTag("Player") && !_playerCollision)
        {
            _playerCollision = true;
            Debug.Log("There is a collision between Player and " + other.gameObject.name);
            // They will turn red.
            _meshRenderer.material.color = Color.red;

            if (_gameplay != null)
            {
                // Maybe it is a good idea to count how many will turn red during our gameplay
                _gameplay.Score += 1;
            }
        }
    }
```

What gameplay do we have? That score has some meaningful use? The simple answer is NO. Take a look `GamePlay.cs`

```c-sharp
public class Gameplay : MonoBehaviour
{
    // We need some internal state for our game
    private int _score = 0;
    public int _maxScore = 0;

    // And we need to shape how this state will be notified...
    public delegate void ScoreChange(int score);
    public delegate void MaxScoreChange(int score); // we could remove it so far we are only interested on the Shape or the type. And this is simple enough.

    // ...by subscribing to some events that we provide
    public event ScoreChange scoreChanged;
    public event MaxScoreChange maxScoreChanged;

    // We expose some our internal state...
    public int Score
    {
        get { return _score; }
        set
        {
            // ... we take care when some procedures happens, and notify all the parties subscribed
            scoreChanged?.Invoke(value);
            _score = value;
        }
    }

    public int MaxScore
    {
        get { return _maxScore; }
        set
        {
            maxScoreChanged?.Invoke(value);
            _maxScore = value;
        }
    }
    //...
```

Amazing, but do we have an object that would care about this score? Yes, the user. But the user won't be looking at the debub console to look for a score. We need to show them the score on screen.

```c-sharp
public class UIScore : MonoBehaviour
{

    // This usually done with a singleton pattern, in this case will pass directly the reference
    [SerializeField]
    private Gameplay _gameplay;

    // Our label where the puntuation is
    private TMPro.TextMeshProUGUI _text;

    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<TMPro.TextMeshProUGUI>();
        if (_text == null)
        {
            Debug.LogError("No TextMeshProUGUI component found");
            return;
        }

        if (_gameplay == null)
        {
            Debug.LogError("We wont be able to update the score at the UI, no Gameplay reference");
        }

        // KEY MOMENT: We subscribe our method to the `Gameplay` events.
        _gameplay.scoreChanged += UpdateScores;
        _gameplay.maxScoreChanged += UpdateScores;
    }


    // We dont really need _score, in this case because we keep it simple, but in other cases a better isolation around the score properties could be done.
    void UpdateScores(int _score)
    {
        _text.text = _gameplay.Score.ToString() + " / " + _gameplay.MaxScore.ToString();
    }
```

That's it. We could keep adding more gameplay like reset the level, once all objects are turned red. Make the follower turn off those objects... Only your imagination is your limit here.

![](.output2.mp4)
fdsfasdf

<video src="output2.mp4" controls title="Title"></video>

In next post we will improve how to manage this Gameplay object to avoid manually reference it.

[Repo: https://github.com/EleDiaz/FDV_MRU](https://github.com/EleDiaz/FDV_MRU)
