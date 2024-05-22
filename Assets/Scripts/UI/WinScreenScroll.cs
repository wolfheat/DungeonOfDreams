using System.Collections;
using TMPro;
using UnityEngine;
using Wolfheat.Inputs;
using Wolfheat.StartMenu;

public class WinScreenScroll : MonoBehaviour
{
    [SerializeField] UIController UIController;
    [SerializeField] TextMeshProUGUI winTimeText;
    [SerializeField] GameObject panel;
    [SerializeField] GameObject scroll;
    private float StartPosition = -1200f;
    private float EndPosition = 5600;
    private float speed = 80f;
    private const float Speedup = 6f;

    public void Show()
    {
        panel.SetActive(true);

        Debug.Log("Win screen Active Pause game");
        StartCoroutine(Animate());
        SoundMaster.Instance.PlayMusic(MusicName.CreditsMusic);
    }

    private IEnumerator Animate()
    {
        RectTransform rect = scroll.GetComponent<RectTransform>();
        Vector2 pos = new Vector3(0, StartPosition);
        rect.anchoredPosition = pos;
        while (pos.y<EndPosition) {         
            yield return null;
            float animationSpeed = Inputs.Instance.Controls.Player.Click.IsPressed() ? speed * Speedup : speed;
            pos += Vector2.up * animationSpeed * Time.unscaledDeltaTime;
            rect.anchoredPosition = pos;
        }
        Debug.Log("Animation of End Credits complete");
        UIController.Instance.ToMainMenu();
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    internal void SetCompleteTimeText(string winTime) => winTimeText.text = winTime;
    /*

<b>Credits
</b>

A game by wolfheat


A variety of assets were used to create this game.


<b>Graphics
</b>

3D
Leartes Studios - Will's Room Environment
RRFreelance - Tools Pack 1
Polygon  - Farm Pack
Pure Poly - Ultimate Low Poly Mining

2D
Vector Dividers Collection
upklyak (freepik) - Ground cracks on land isolated
GarryKillian(freepik) - Wallpaper
masadepan(freepik) - Vector Compass
Zooperdan - Some logos
João Baltieri - Mini Simple Characters | Skeleton

VFX
Unity Technologies - Particle Pack
Vefects - Item Pickup VFX


<b>Audio
</b>

Freesound - Various Sounds
Uppbeat - Music

Vasyl Sakal - Celestial Lullaby 
Brock Hewitt - In The Silence


<b>Fonts
</b>

Prida01 - gluk


<b>AI Tools
</b>

Genny - Voices - Cloe, Matthew

Playground AI - 2D art


<b>Other Tools Used
</b>

Audacity

Gimp

Blender

Unity


<b>Game Jam
</b>

This game was created for the
Dungeon Crawler Game Jam 2024


Thank you for playing!



*/
}
