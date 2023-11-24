﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TDTK {
	
	[CreateAssetMenu(fileName = "CreepDB", menuName = "TDTK_DB/CreepDB", order = 1)]
	public class Creep_DB : ScriptableObject {
		
		[HideInInspector] public List<GameObject> objList=new List<GameObject>();
		public List<UnitCreep> creepList=new List<UnitCreep>();
		
		public static Creep_DB LoadDB(){
			return Resources.Load("DB/CreepDB", typeof(Creep_DB)) as Creep_DB;
		}
		
		#region runtime code
		public static Creep_DB instance;
		public static Creep_DB Init(){
			if(instance!=null) return instance;
			instance=LoadDB();
			
			#if UNITY_2018_3_OR_NEWER && UNITY_EDITOR
			instance.FillObjectList();
			#endif
			
			UpdateLabel();
			return instance;
		}
	
		
		public static Creep_DB GetDB(){ return Init(); }
		public static List<UnitCreep> GetList(bool verify=true){
			Init();
			if(verify) VerifyList();
			return instance.creepList;
		}
		public static UnitCreep GetItem(int index){ Init(); return (index>=0 && index<instance.creepList.Count) ? instance.creepList[index] : null; }
		
		public static void VerifyList(){
			#if UNITY_2018_3_OR_NEWER
			for(int i=0; i<instance.creepList.Count; i++){
				if(instance.creepList[i]!=null){
					if(instance.objList.Count>i)
						instance.objList[i]=instance.creepList[i].gameObject;
					else
						instance.objList.Add(instance.creepList[i].gameObject);
					continue;
				}
				
				if(i<instance.objList.Count && instance.objList[i]!=null){
					UnitCreep creep=instance.objList[i].GetComponent<UnitCreep>();
					if(creep!=null){
						instance.creepList[i]=creep;
						continue;
					}
				}
				
				instance.creepList.RemoveAt(i);	i-=1;
			}
			
			while(instance.objList.Count>instance.creepList.Count){
				instance.objList.RemoveAt(instance.objList.Count-1);
			}
			#else
			for(int i=0; i<instance.creepList.Count; i++){
				if(instance.creepList[i]==null){ instance.creepList.RemoveAt(i);	i-=1; }
			}
			#endif
		}
		
		public static void CheckPrefabID(){
			List<int> pIDList=new List<int>();
			for(int i=0; i<instance.creepList.Count; i++){
				if(pIDList.Contains(instance.creepList[i].prefabID)){
					Debug.LogWarning("Creep prefab - "+instance.creepList[i].gameObject.name+" is using a prefabID used by other creep prefab");
				}
				else pIDList.Add(instance.creepList[i].prefabID);
			}
		}
		
		public static List<int> GetPrefabIDList(){
			Init();
			List<int> prefabIDList=new List<int>();
			for(int i=0; i<instance.creepList.Count; i++) prefabIDList.Add(instance.creepList[i].prefabID);
			return prefabIDList;
		}
		
		public static int GetPrefabIndex(int pID){
			Init();
			for(int i=0; i<instance.creepList.Count; i++){
				if(instance.creepList[i].prefabID==pID) return i;
			}
			return -1;
		}
		public static int GetPrefabIndex(UnitCreep creep){
			if(creep==null) return -1;
			return GetPrefabIndex(creep.prefabID);
		}
		
		public static string[] label;
		public static void UpdateLabel(){
			label=new string[GetList(false).Count];
			for(int i=0; i<label.Length; i++) label[i]=i+" - "+GetItem(i).unitName;
		}
		#endregion
		
		
		#if UNITY_EDITOR
		[ContextMenu ("Reset PrefabID")]
		public void ResetPrefabID(){
			for(int i=0; i<creepList.Count; i++){
				creepList[i].prefabID=i;
				UnityEditor.EditorUtility.SetDirty(creepList[i]);
			}
		}
		
		//[ContextMenu ("Fill ObjectList")]
		public void FillObjectList(){
			objList=new List<GameObject>();
			for(int i=0; i<creepList.Count; i++) objList.Add(creepList[i].gameObject);
		}
		#endif
		
		
		//[HideInInspector]
		[Space(10)]
		public bool copiedFromOldDB=false;
		public static void CopyFromOldDB(){		Init();
			if(instance.copiedFromOldDB) return;
			
			instance.copiedFromOldDB=true;
			instance.creepList=new List<UnitCreep>( CreepDB.GetList1() );
		}
		public static bool UpdatedToPost_2018_3(){ Init(); return instance.copiedFromOldDB; }
	}

}
