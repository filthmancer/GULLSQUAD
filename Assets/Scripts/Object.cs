using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gull
{
    public class Object : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public virtual void InteractWith<T>(T obj) where T : Gull.Object
        {

        }

        public virtual void ExitInteract<T>(T obj) where T : Gull.Object
        {

        }


        void OnTriggerEnter2D(Collider2D col)
        {
            Gull.Object obj = col.transform.GetComponent<Gull.Object>();
            if (obj != null)
            {
                obj.InteractWith(this);
                this.InteractWith(obj);
            }
        }

        void OnTriggerExit2D(Collider2D col)
        {
            Gull.Object obj = col.transform.GetComponent<Gull.Object>();
            if (obj != null)
            {
                obj.ExitInteract(this);
                this.InteractWith(obj);
            }
        }

    }
}
