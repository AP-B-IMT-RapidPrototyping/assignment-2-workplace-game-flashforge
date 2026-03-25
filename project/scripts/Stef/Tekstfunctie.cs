using Godot;
using System;
using System.Collections.Generic;

public partial class Tekstfunctie : RichTextLabel
{
    [Export] public float LettersPerSeconde = 20.0f;
    [Export] public float PauzeEindeZin = 1.0f;

    private Tween _activeTween;

    public override void _Ready()
    {
        VisibleCharacters = 0;
    }

    public async void SpeelTekstAf(string volledigeTekst)
    {
        if (_activeTween != null && _activeTween.IsRunning())
        {
            _activeTween.Kill();
        }
        Text = volledigeTekst;
        VisibleCharacters = 0;
        string[] zinnen = volledigeTekst.Split(new[] { '.', '?', '!' }, StringSplitOptions.RemoveEmptyEntries);
        int huidigeZichtbareKarakters = 0;

        foreach (string zin in zinnen)
        {
            int zinLengte = zin.Length + 1;
            int targetKarakters = huidigeZichtbareKarakters + zinLengte;
            
            targetKarakters = Mathf.Min(targetKarakters, Text.Length);

            float duur = zinLengte / LettersPerSeconde;

            _activeTween = CreateTween();
            _activeTween.SetTrans(Tween.TransitionType.Linear);
            
            _activeTween.TweenProperty(this, "visible_characters", targetKarakters, duur)
                .From(huidigeZichtbareKarakters);

            await ToSignal(_activeTween, "finished");
            
            huidigeZichtbareKarakters = targetKarakters;

            if (huidigeZichtbareKarakters < Text.Length)
            {
                GD.Print("Pauze einde zin...");
                await ToSignal(GetTree().CreateTimer(PauzeEindeZin), "timeout");
            }
        }
        
        GD.Print("Typewriter effect voltooid.");
    }
}