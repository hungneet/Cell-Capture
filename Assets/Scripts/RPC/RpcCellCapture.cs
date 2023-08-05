using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Netcode.Samples
{
    public class RpcCellCapture : NetworkBehaviour
    {
        // Start is called before the first frame update
/*        public InputField mess;
        public Text chat;
*/
        


        // Update is called once per frame
        void Update()
        {
/*            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    updateMessageClientRpc(mess.text);
                }
                else if (NetworkManager.Singleton.IsClient)
                {
                    sendMessageServerRpc(mess.text);
                }
            }*/

        }
/*
        [ServerRpc]
        public void sendMessageServerRpc(string message)
        {
            updateMessageClientRpc(message);
        }

        [ClientRpc]
        public void updateMessageClientRpc(string message)
        {
            chat.text += message + "\n";
        }*/




    }
}
