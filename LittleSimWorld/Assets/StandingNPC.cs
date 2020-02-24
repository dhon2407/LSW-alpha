using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.RandomNPC
{
    public class StandingNPC : RandomNPC
    {
        public CharacterOrientation StartOrientation;
        // Start is called before the first frame update
        void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            commandQueue = new Queue<INPCCommand>();

            col.isTrigger = false;

         

            visualsHelper.Initialize();
            visualsHelper.UpdateVisuals(StartOrientation);
            CalculateFacing(FacingVectorNorm());
        }

        public Vector3 FacingVectorNorm()
        {
            switch (StartOrientation)
            {
                case CharacterOrientation.Bot:
                    return new Vector3(0, -1, 0);
                case CharacterOrientation.Top:
                    return new Vector3(0, 1, 0);
                case CharacterOrientation.Right:
                    return new Vector3(1, 0, 0);
                case CharacterOrientation.Left:
                    return new Vector3(-1, 0, 0);
                default:
                    return new Vector3(0, 0, 0);
            }
        }
        public void CalculateFacing(Vector2 offset)
        {

            bool isYBigger = Mathf.Abs(offset.x) <= 0.01f;//Mathf.Abs(offset.x) < Mathf.Abs(offset.y);

            if (offset == Vector2.zero) { return; }

            if (isYBigger)
            {
                if (offset.y > 0) { visualsHelper.FaceUP(); }
                else if (offset.y < 0) { visualsHelper.FaceDOWN(); }

                if (offset.y != 0) { anim.SetFloat("Vertical", Mathf.Sign(offset.y)); }
                else { anim.SetFloat("Vertical", 0); }

                anim.SetFloat("Horizontal", 0);
            }
            else
            {
                if (offset.x > 0) { visualsHelper.FaceRIGHT(); }
                else if (offset.x < 0) { visualsHelper.FaceLEFT(); }

                if (offset.x != 0) { anim.SetFloat("Horizontal", Mathf.Sign(offset.x)); }
                else { anim.SetFloat("Horizontal", 0); }

                anim.SetFloat("Vertical", 0);
            }
        }
    }
}
