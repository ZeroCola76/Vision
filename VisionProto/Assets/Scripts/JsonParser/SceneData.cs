using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class MeshComponentConverter : JsonConverter<MeshComponent>
{
    public override void WriteJson(JsonWriter writer, MeshComponent value, JsonSerializer serializer)
    {
        JObject jo = new JObject
        {
            { "FBX", new JArray(value.FBXAsIntArray) },
            { "FBXFilter", value.FBXFilter },
            { "ComponentID", value.ComponentID }
        };
        jo.WriteTo(writer);
    }

    public override MeshComponent ReadJson(JsonReader reader, Type objectType, MeshComponent existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);
        MeshComponent mc = new MeshComponent
        {
            FBXAsIntArray = jo["FBX"].ToObject<int[]>(),
            FBXFilter = jo["FBX"].ToString(),
            ComponentID = jo["ComponentID"].ToObject<uint>()
        };
        return mc;
    }
}




[System.Serializable]
public class SceneData
{
    public List<EntityData> Entitys = new List<EntityData>();
    public PhysicsInfo PhysicsInfo = new PhysicsInfo();
}

[System.Serializable]
public class EntityData
{
    public uint EntityID;
    public List<ComponentData> Component = new List<ComponentData>();
}

[System.Serializable]
public class SimpleVector3
{
    public float x;
    public float y;
    public float z;

    public SimpleVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public SimpleVector3(Vector3 vector)
    {
        this.x = vector.x;
        this.y = vector.y;
        this.z = vector.z;
    }
}

[System.Serializable]
public class SimpleQuaternion
{
    public float x;
    public float y;
    public float z;
    public float w;

    public SimpleQuaternion(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public SimpleQuaternion(Quaternion quaternion)
    {
        this.x = quaternion.x;
        this.y = quaternion.y;
        this.z = quaternion.z;
        this.w = quaternion.w;
    }
}

[System.Serializable]
public class ComponentData
{
    public uint ComponentID;
}

[System.Serializable]
public class IDComponent : ComponentData
{
    public string Name;
    public IDComponent()
    {
        ComponentID = 1365953231;
    }
}

[System.Serializable]
public class TransformComponent : ComponentData
{
    public SimpleVector3 Local_Location;
    public SimpleQuaternion Local_Quaternion;
    public SimpleVector3 Local_Scale;
    public TransformComponent()
    {
        ComponentID = 3400599862;
        Local_Location = new SimpleVector3(0, 0, 0);
        Local_Quaternion = new SimpleQuaternion(0, 0, 0, 1);
        Local_Scale = new SimpleVector3(1, 1, 1);
    }
}
[System.Serializable]
public class MeshComponent : ComponentData
{
    public string FBX;
    public string FBXFilter = "Static";

    public MeshComponent()
    {
        ComponentID = 3774279003;
        FBX = "";
        FBXFilter = "Static";
    }

    [JsonIgnore]
    public int[] FBXAsIntArray
    {
        get
        {
            int[] intArray = new int[FBX.Length];
            for (int i = 0; i < FBX.Length; i++)
            {
                intArray[i] = FBX[i];
            }
            return intArray;
        }
        set
        {
            char[] charArray = new char[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                charArray[i] = (char)value[i];
            }
            FBX = new string(charArray);
        }
    }
}

[System.Serializable]
public class ParentComponent : ComponentData
{
    public uint ParentID;
    public ParentComponent()
    {
        ComponentID = 3243699741;
        ParentID = 0;
    }
}

[System.Serializable]
public class ChildComponent : ComponentData
{
    public List<uint> ChildrenID = new List<uint>();
    public ChildComponent()
    {
        ComponentID = 188567026;
    }
}

[System.Serializable]
public class RigidBodyComponent : ComponentData
{
    public uint ColliderType = 1;
    public uint ColliderShape = 3;
    public bool IsDynamic = false;
    public BoxColliderInfo BoxInfo = new BoxColliderInfo();
    public SphereColliderInfo SphereInfo = new SphereColliderInfo();
    public CapsuleColliderInfo CapsuleInfo = new CapsuleColliderInfo();
    public ColliderInfo DefaultColliderInfo = new ColliderInfo();

    public RigidBodyComponent()
    {
        ComponentID = 4217231799;
        BoxInfo.Extent = new SimpleVector3(1, 1, 1);
    }
}

[System.Serializable]
public class ColliderInfo
{
    public bool[] AngleLock = new bool[3] { false, false, false };
    public float Density = 1.0f;
    public float DynamicFriction = 1.0f;
    public bool[] LinearLock = new bool[3] { false, false, false };
    public SimpleVector3 OffSet = new SimpleVector3(0, 0, 0);
    public int PhysicsLayer = 0;
    public float Restitution = 0.0f;
    public float StaticFriction = 1.0f;
    public bool UseGravity = false;
}

[System.Serializable]
public class BoxColliderInfo
{
    public SimpleVector3 Extent = new SimpleVector3(1, 1, 1);
}

[System.Serializable]
public class CapsuleColliderInfo
{
    public float HalfHeight;
    public float Radius;
}

[System.Serializable]
public class SphereColliderInfo
{
    public float Radius = 0.5f;
}

[System.Serializable]
public class PhysicsInfo
{
    public List<int> CollisionMatrix = new List<int>();
    public int FrameRate = 60;
    public SimpleVector3 Gravity = new SimpleVector3(0, 0, 0);

    public PhysicsInfo()
    {
        for (int i = 0; i < (int)EPhysicsLayer.END; i++)
        {
            CollisionMatrix.Add(0);
        }
    }
}

public enum EPhysicsLayer
{
    GROUND = 0,
    WALL,
    TOP,
    PLAYER,
    OBJECT,
    ENEMY,
    DOOR,
    ACTIVEDOOR,
    TRIGGER,
    END
}
