# Avanzando en unity MRU

![](scene.png)

Movamos objetos en nuestra escena. Uno de ellos jugará al pilla-pilla y el otro será manejado por el usuario. Para obtener este resultado abusaremos de la siguiente fórmula, la cual representa la posición del objeto a lo largo del tiempo dada una velocidad.

```csharp
position = position_0 + velocity * time_lapse
```

- `position_0` será nuestra posición inicial o previa, la cual puede ser vector 2d o 3d en nuestro caso.
- `time_lapse` será la diferencia entre el instante de tiempo en el que fue calculada el valor de `position_0` y el tiempo actual. También es conocido "delta time". Que en nuestro caso sera el tiempo que tarda el motor en generar cada frame de render.

```csharp
// CharacterMovement.cs
void Update()
{
    // Obtenemos la entrada del usuario a partir del `movementAction` que es un `InputActionReference`
    var direction = movementAction.action.ReadValue<Vector2>();
    if (direction != Vector2.zero)
    {
        Vector3 movement = new Vector3(direction.x, 0f, direction.y);
        // Aplicamos la ya mencionada formula.
        transform.Translate(movement * Time.deltaTime * speed, Space.Self);
    }

    var look = lookAround.action.ReadValue<Vector2>();
    if (look != Vector2.zero)
    {
        Vector3 rotation = new Vector3(0f,look.x, 0f);
        // También podemos usar la misma formula para calcular nuevas rotaciones.
        transform.Rotate(rotation * Time.deltaTime * rotationSpeed, Space.Self);
    }
}
```

En el caso del objeto que nos seguirá. La dirección no vendrá dada por el usuario, sino en cambio la obtendremos por medio de la relación vectorial entre que une el persecutor con el player. Es decir, la diferencia de sus posiciones nos dará el vector, que normalizaremos para obtener la dirección a la que dirirgir este objeto.

```csharp
// SimpleFollow.cs
var direction = target.transform.position - transform.position;
// ...
// To later apply the translation. DON'T forget to normalized the direction.
transform.Translate(direction.normalized * Time.deltaTime * speed, Space.World);
```

Aún esto, no es lo suficientemente divertido. Añadamos unos obstáculos, para dar sensación de dificultad. Y, un objetivo.

Empecemos por añadir unos obstáculos en la escena.

```csharp
// GenerateMap.cs
for (int i = 0; i < amount; i++)
{
    // Dejemos un espacio alrededor del jugador `innerRadius`, pero con límites `outerRadius`. No se trata de hacer un No Man's Sky.
    var distance = Random.Range(innerRadius, outerRadius);
    // Elijamos un ángulo al cual "lanzaremos" nuestro obstáculo.
    var angle = Random.Range(0, 360f);
    // Con un poco de magia Quaterniana rotaremos el vector desde la posición inicial usando los valores anteriores.
    var position = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * distance + initialOrigin.position;

    // Construimos nuestro mundo.
    var gameObject = Instantiate(obstacle, position, Quaternion.identity);
    gameObject.name = "Obstacle " + i;
    // ...
}
```

![](./map.png)

Sigue sin ser suficiente. Son solamente objetos en posiciones aleatorias sin interacción. El jugador tendrá que darles vida.

```csharp
    // PlayerReaction.cs
    void OnCollisionEnter(Collision other)
    {
        // Cuando el jugador toca esstos objetos reaccionaremos.
        if (other.gameObject.CompareTag("Player") && !_playerCollision)
        {
            _playerCollision = true;
            Debug.Log("There is a collision between Player and " + other.gameObject.name);
            // Y se pondrán rojos.
            _meshRenderer.material.color = Color.red;

            if (_gameplay != null)
            {
                // Quizás sea buena idea llevar la cuenta de cuantos se han puestos rojos durante nuestro juego.
                _gameplay.Score += 1;
            }
        }
    }
```

¿De que va el juego? ¿La puntuación tiene algún uso? No, pero echemos un vistazo a `Gameplay.cs`

```csharp
public class Gameplay : MonoBehaviour
{
    // El estado de nuestro juego
    private int _score = 0;
    public int _maxScore = 0;

    // Y le damos forma a como el estado será notificado...
    public delegate void ScoreChange(int score);
    public delegate void MaxScoreChange(int score);

    // ...que será por medio de subscripciones a eventos que proveemos.
    public event ScoreChange scoreChanged;
    public event MaxScoreChange maxScoreChanged;

    // Exponemos nuestro estado interno...
    public int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            // ... pero notificaremos de cuando algunos procedimientos ocurren.
            scoreChanged?.Invoke(value);
        }
    }

    public int MaxScore
    {
        get { return _maxScore; }
        set
        {
            _maxScore = value;
            maxScoreChanged?.Invoke(value);
        }
    }
    //...
```

Increíble, pero ¿hay algo al cual le interese saber esta puntuación? Si, el usuario. Pero este no estára mirando a la consola de depuracion para ver esta puntuación. Por ello le mostraremos la puntuación en pantalla.

```csharp
public class UIScore : MonoBehaviour
{

    // Esto se debería de realizar por medio de un `Singleton Pattern`, pero en este caso lo haremos manualmente
    [SerializeField]
    private Gameplay _gameplay;

    // Nuestra etiqueta donde la puntuación se halla.
    private TMPro.TextMeshProUGUI _text;

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

        // MOMENTO CLAVE: Subscribimos nuestro objeto a los eventos de `Gameplay`
        _gameplay.scoreChanged += UpdateScores;
        _gameplay.maxScoreChanged += UpdateScores;
    }


    // Realmente no tenemos necesidad de pasar score por el evento, pues el estado esta completamente expuesto.
    // Pero en otros casos el aislamiento del estado puede ser beneficioso
    void UpdateScores(int _score)
    {
        _text.text = _gameplay.Score.ToString() + " / " + _gameplay.MaxScore.ToString();
    }
```

Y eso es todo. Podríamos añadir mas dinámicas en el juego como resetearlo cuando todos los objetos se ponen rojos. Hacer que el seguidor apague los objetos. Solo estamos limitados por nuestra imaginación llegados aquí.


https://github.com/EleDiaz/FDV_MRU/assets/2550542/c9cc6ca6-4cca-431d-910e-3991189d237d


En el siguiente post mejoraremos como tratar con este objeto `Gameplay` y evitar su referenciación manual.

[Repo: https://github.com/EleDiaz/FDV_MRU](https://github.com/EleDiaz/FDV_MRU)
