using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMessages : MonoBehaviour
{
    int opcode;
    string username;
    string password;

    public static ItemMessages Instance { get; set; }


    public ItemMessages newItemMessages(int opcode, string username, string password)
    {
        this.opcode = opcode;
        this.username = username;
        this.password = password;
        return this;
    }
}


