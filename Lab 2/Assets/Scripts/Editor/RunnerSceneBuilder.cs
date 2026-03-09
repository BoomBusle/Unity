using UnityEngine;
using UnityEditor;

public class RunnerSceneBuilder : EditorWindow
{
    static float trackWidth = 10f;

    [MenuItem("Tools/Створити рівень (Лаб 2)")]
    static void BuildScene()
    {
        if (!EditorUtility.DisplayDialog("Лаб 2",
            "Створити полосу перешкод з гравцем?", "Створити", "Скасувати"))
            return;

        CreateTags();

        GameObject root = new GameObject("Level");
        Undo.RegisterCreatedObjectUndo(root, "Create Level");

        CreateTrackSegment("Track_1", new Vector3(0f, -0.5f, 25f), 50f, root.transform);
        CreateTrackSegment("Track_2", new Vector3(0f, -0.5f, 70f), 20f, root.transform);
        CreateTrackSegment("Track_3", new Vector3(0f, -0.5f, 115f), 50f, root.transform);
        CreateTrackSegment("Track_4", new Vector3(0f, -0.5f, 160f), 20f, root.transform);
        CreateTrackSegment("Track_5", new Vector3(0f, -0.5f, 195f), 30f, root.transform);

        CreateWall("Wall_Left", new Vector3(-5.5f, 1f, 105f), new Vector3(0.5f, 3f, 210f), root.transform);
        CreateWall("Wall_Right", new Vector3(5.5f, 1f, 105f), new Vector3(0.5f, 3f, 210f), root.transform);

        GameObject obstParent = new GameObject("Obstacles_ACF");
        obstParent.transform.SetParent(root.transform);

        GameObject platA = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platA.name = "A_MovingPlatform";
        platA.tag = "Obstacle";
        platA.transform.position = new Vector3(-3f, 0.5f, 30f);
        platA.transform.localScale = new Vector3(3f, 1f, 2f);
        platA.transform.SetParent(obstParent.transform);
        MovingPlatform mp = platA.AddComponent<MovingPlatform>();
        mp.pointA = new Vector3(-3f, 0.5f, 30f);
        mp.pointB = new Vector3(3f, 0.5f, 30f);
        mp.speed = 3f;
        SetColor(platA, new Color(1f, 0.6f, 0f));

        GameObject pendulumFrame = new GameObject("C_PendulumFrame");
        pendulumFrame.transform.SetParent(obstParent.transform);
        CreateStaticBlock("PillarL", new Vector3(-2.5f, 2.5f, 65f), new Vector3(0.3f, 5f, 0.3f), pendulumFrame.transform, new Color(0.5f, 0.5f, 0.5f));
        CreateStaticBlock("PillarR", new Vector3(2.5f, 2.5f, 65f), new Vector3(0.3f, 5f, 0.3f), pendulumFrame.transform, new Color(0.5f, 0.5f, 0.5f));
        CreateStaticBlock("TopBar", new Vector3(0f, 5.2f, 65f), new Vector3(5.5f, 0.3f, 0.3f), pendulumFrame.transform, new Color(0.5f, 0.5f, 0.5f));

        GameObject pendulum = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pendulum.name = "C_Pendulum";
        pendulum.tag = "Obstacle";
        pendulum.transform.position = new Vector3(0f, 2f, 65f);
        pendulum.transform.localScale = new Vector3(0.5f, 4f, 0.5f);
        pendulum.transform.SetParent(obstParent.transform);
        PendulumObstacle po = pendulum.AddComponent<PendulumObstacle>();
        po.maxAngle = 50f;
        po.swingSpeed = 2.5f;
        po.armLength = 3f;
        SetColor(pendulum, new Color(0.8f, 0.2f, 0.2f));

        GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cyl.name = "F_AstroidCylinder";
        cyl.tag = "Obstacle";
        cyl.transform.position = new Vector3(0f, 1f, 120f);
        cyl.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        cyl.transform.SetParent(obstParent.transform);
        AstroidCylinder ac = cyl.AddComponent<AstroidCylinder>();
        ac.radius = 3f;
        ac.speed = 1.5f;
        ac.centerOffset = new Vector3(0f, 1f, 120f);
        SetColor(cyl, new Color(0.9f, 0.2f, 0.9f));

        GameObject extraObst = new GameObject("ExtraObstacles");
        extraObst.transform.SetParent(root.transform);

        CreateObstacle("Obstacle_1", new Vector3(3f, 0.5f, 15f), new Vector3(2f, 1f, 1f), extraObst.transform);
        CreateObstacle("Obstacle_2", new Vector3(-3f, 0.5f, 45f), new Vector3(2f, 1.2f, 1f), extraObst.transform);
        CreateObstacle("Obstacle_3", new Vector3(0f, 0.5f, 100f), new Vector3(2f, 1f, 1f), extraObst.transform);
        CreateObstacle("Obstacle_4", new Vector3(3f, 0.75f, 110f), new Vector3(2f, 1.5f, 1f), extraObst.transform);
        CreateObstacle("Obstacle_5", new Vector3(-3f, 0.5f, 155f), new Vector3(2f, 1f, 1f), extraObst.transform);
        CreateObstacle("Obstacle_6", new Vector3(0f, 0.5f, 175f), new Vector3(2.5f, 1.2f, 1f), extraObst.transform);
        CreateObstacle("Obstacle_7", new Vector3(3f, 0.5f, 190f), new Vector3(2f, 1f, 1f), extraObst.transform);

        GameObject finish = GameObject.CreatePrimitive(PrimitiveType.Cube);
        finish.name = "Finish";
        finish.tag = "Finish";
        finish.transform.position = new Vector3(0f, 2f, 209f);
        finish.transform.localScale = new Vector3(trackWidth, 4f, 0.5f);
        finish.transform.SetParent(root.transform);
        SetColor(finish, new Color(0.1f, 0.9f, 0.1f));

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(0f, 1f, 0f);
        CharacterController cc = player.AddComponent<CharacterController>();
        cc.height = 2f;
        cc.radius = 0.5f;
        player.AddComponent<PlayerRunner>();
        SetColor(player, new Color(0.2f, 0.5f, 1f));
        Undo.RegisterCreatedObjectUndo(player, "Create Player");

        if (Camera.main != null)
        {
            CameraFollow cam = Camera.main.gameObject.AddComponent<CameraFollow>();
            cam.target = player.transform;
        }

        if (Object.FindObjectOfType<Light>() == null)
        {
            GameObject light = new GameObject("DirectionalLight");
            Light l = light.AddComponent<Light>();
            l.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            Undo.RegisterCreatedObjectUndo(light, "Create Light");
        }

        EditorUtility.DisplayDialog("Готово!",
            "Рівень створено!\n\nA/D - вліво/вправо\nПробіл - стрибок\nShift - прискорення (макс 3 сек)\n\nПерешкоди з Лаб 1: A (плита), C (маятник), F (циліндр)\n+ додаткові перешкоди, ями, фініш", "OK");
    }

    static void CreateTags()
    {
        AddTag("Obstacle");
        AddTag("Finish");
    }

    static void AddTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        for (int i = 0; i < tagsProp.arraySize; i++)
            if (tagsProp.GetArrayElementAtIndex(i).stringValue == tag) return;
        tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
        tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
        tagManager.ApplyModifiedProperties();
    }

    static void CreateTrackSegment(string name, Vector3 pos, float length, Transform parent)
    {
        GameObject seg = GameObject.CreatePrimitive(PrimitiveType.Cube);
        seg.name = name;
        seg.transform.position = pos;
        seg.transform.localScale = new Vector3(trackWidth, 1f, length);
        seg.transform.SetParent(parent);
        seg.isStatic = true;
        SetColor(seg, new Color(0.3f, 0.3f, 0.35f));
    }

    static void CreateObstacle(string name, Vector3 pos, Vector3 scale, Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.tag = "Obstacle";
        obj.transform.position = pos;
        obj.transform.localScale = scale;
        obj.transform.SetParent(parent);
        SetColor(obj, new Color(0.9f, 0.2f, 0.2f));
    }

    static void CreateStaticBlock(string name, Vector3 pos, Vector3 scale, Transform parent, Color color)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.position = pos;
        obj.transform.localScale = scale;
        obj.transform.SetParent(parent);
        obj.isStatic = true;
        SetColor(obj, color);
    }

    static void CreateWall(string name, Vector3 pos, Vector3 scale, Transform parent)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.position = pos;
        wall.transform.localScale = scale;
        wall.transform.SetParent(parent);
        wall.isStatic = true;
        SetColor(wall, new Color(0.4f, 0.4f, 0.4f));
    }

    static void SetColor(GameObject obj, Color color)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                shader = Shader.Find("Standard");
            Material mat = new Material(shader);
            mat.SetColor("_BaseColor", color);
            mat.color = color;
            renderer.sharedMaterial = mat;
        }
    }
}
