#region Imports

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Threading;
using Messages;
using Messages.custom_msgs;
using Ros_CSharp;
using XmlRpc_Wrapper;
using Int32 = Messages.std_msgs.Int32;
using String = Messages.std_msgs.String;
using m = Messages.std_msgs;
using gm = Messages.geometry_msgs;
using nm = Messages.nav_msgs;
using sm = Messages.sensor_msgs;
using System.Text;

#endregion

public class NewBehaviourScript : MonoBehaviour {

	NodeHandle node;
	Publisher<m.String> Talker;
	Subscriber<Messages.std_msgs.String> sub;
	int count = 0;

	// Use this for initialization
	void Start () {
		ROS.ROS_MASTER_URI = "http://127.0.0.1:11311/";
		string[] args = new string[0];
		ROS.Init(args, "Talker");
		node = new NodeHandle();
		Talker = node.advertise<m.String>("/chatter", 1000);

		sub = node.subscribe<Messages.std_msgs.String>("/chatter_to_unity", 10, subCallback);

		StartCoroutine(Coro());

	}

	public void subCallback(Messages.std_msgs.String msg)
	{
		ROS.Info("Receieved:\n" + msg.data);
	}

	
	// Update is called once per frame
	void Update () {

	}

	void OnMouseDown()
	{
		ROS.Info("Publishing a chatter message:    \"Blah blah blah " + count + "\"");
		String pow = new String("Blah blah blah " + (count++));

		Talker.publish(pow);
	}

	private IEnumerator Coro()
	{
		while (true)
		{
			//ROS.Info("Publishing a chatter message:    \"Coro msg " + count + "\"");
			String pow = new String("Coro msg " + (count++));

			Talker.publish(pow);

			yield return new WaitForSecondsRealtime (1f/33f);
		}
	}


}
