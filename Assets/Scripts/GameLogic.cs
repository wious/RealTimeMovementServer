using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    Vector2 characterPositionInPercent;
    Vector2 characterVelocityInPercent;
    const float CharacterSpeed = 0.25f;
    float HalfCharacterSpeed = Mathf.Sqrt(CharacterSpeed * CharacterSpeed + CharacterSpeed * CharacterSpeed) / 2f;  //   0.125f;

    void Start()
    {
        NetworkedServerProcessing.SetGameLogic(this);
    }

    void Update()
    {
        characterPositionInPercent += (characterVelocityInPercent * Time.deltaTime);

        Debug.Log(characterPositionInPercent);
    }

    public void UpdateDirectionalInput(int d, int clientID)
    {
        characterVelocityInPercent = Vector2.zero;

        if (d == KeyboardInputDirections.UpRight)
        {
            characterVelocityInPercent.x = HalfCharacterSpeed;
            characterVelocityInPercent.y = HalfCharacterSpeed;
        }
        else if (d == KeyboardInputDirections.UpLeft)
        {
            characterVelocityInPercent.x = -HalfCharacterSpeed;
            characterVelocityInPercent.y = HalfCharacterSpeed;
        }
        else if (d == KeyboardInputDirections.DownRight)
        {
            characterVelocityInPercent.x = HalfCharacterSpeed;
            characterVelocityInPercent.y = -HalfCharacterSpeed;
        }
        else if (d == KeyboardInputDirections.DownLeft)
        {
            characterVelocityInPercent.x = -HalfCharacterSpeed;
            characterVelocityInPercent.y = -HalfCharacterSpeed;
        }
        else if (d == KeyboardInputDirections.Right)
        {
            characterVelocityInPercent.x = CharacterSpeed;
        }
        else if (d == KeyboardInputDirections.Left)
        {
            characterVelocityInPercent.x = -CharacterSpeed;
        }
        else if (d == KeyboardInputDirections.Up)
        {
            characterVelocityInPercent.y = CharacterSpeed;
        }
        else if (d == KeyboardInputDirections.Down)
        {
            characterVelocityInPercent.y = -CharacterSpeed;
        }
        else if (d == KeyboardInputDirections.NoPress)
        {

        }

        NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.VelocityAndPosition + ","
        + characterVelocityInPercent.x + "," + characterVelocityInPercent.y+","
        + characterPositionInPercent.x + "," + characterPositionInPercent.y, clientID);
    }



}