using UnityEngine;
using TMPro;

public class GameOverBounceText : MonoBehaviour
{
    [Header("Bounce Settings")]
    public TextMeshProUGUI bounceText;
    public float bounceAmount = 0.15f;
    public float bounceSpeed = 2f;

    private Vector3 baseScale;

    private string[] phrases = new string[]
    {
        "That’s it? Prove you can do better.",
        "Is that all you’ve got?",
        "Come on, don’t let the game win!",
        "Your high score is laughing at you.",
        "Afraid to try again?",
        "The neon isn’t going to escape itself!",
        "One more run… unless you’re scared?",
        "You call that a run?",
        "The obstacles are still waiting for you.",
        "Maybe next time you’ll make it… or not.",
        "The retry button is right there. Just saying.",
        "Don’t let the game beat you!",
        "You can quit… or you can get good.",
        "The leaderboard is safe… for now.",
        "That was just a warm-up, right?",
        "Are you going to let the game trash talk you?",
        "You missed that jump on purpose, right?",
        "Come on, show us what you’ve really got!",
        "The neon world isn’t impressed yet.",
        "You’re not gonna let a game beat you, are you?",
        "You died. But at least you didn’t lose to Dark Souls.",
        "Try again, Senpai! Notice your own skills.",
        "It’s dangerous to go alone. Play again!",
        "Do a barrel roll! (But maybe jump next time.)",
        "You were defeated, but your power level can still go over 9000.",
        "Insert coin… or a Dragon Ball wish?",
        "Game Over. But remember: The cake is a lie.",
        "You have no power here… yet.",
        "Retry, you must. (Yoda voice)",
        "Would Goku give up? Didn’t think so.",
        "You lost… but did you at least catch ‘em all?",
        "May the next run be ever in your favor.",
        "It’s super effective! Against your score.",
        "You lost the game. (And now you lost The Game.)",
        "This isn’t even my final form! Try again?",
        "You can’t pause an anime cliffhanger. Why pause now?",
        "Like a true Joestar, get up and run",
        "You’re filled with determination. (Undertale ref)",
        "Press retry. For Frodo.",
        "The neon force is not strong with you… yet."
    };

    void OnEnable()
    {
        if (bounceText != null && phrases.Length > 0)
        {
            string phrase = phrases[Random.Range(0, phrases.Length)];
            bounceText.text = $"\"{phrase}\"";
        }
    }

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float scale = 1f + Mathf.Sin(Time.time * bounceSpeed) * bounceAmount;
        transform.localScale = baseScale * scale;
    }
}
