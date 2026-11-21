using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class GameplayTests
{
    private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
    private Assembly gameAssembly;
    private GameObject gameObject;

    [SetUp]
    public void SetUp()
    {
        gameAssembly = Assembly.Load("Assembly-CSharp");
        gameObject = new GameObject("Gameplay test");
        gameObject.SetActive(false);
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void SprintOnlyUsesStaminaWhileMovingAndRecoversWhileHeldAfterExhaustion()
    {
        Type type = gameAssembly.GetType("GameEntityObject", true);
        Component entity = gameObject.AddComponent(type);
        FieldInfo stamina = type.GetField("_stamina", PrivateInstance);
        FieldInfo moving = type.GetField("_isMoving", PrivateInstance);
        FieldInfo sprinting = type.GetField("_isSprinting", PrivateInstance);
        MethodInfo updateStamina = type.GetMethod("UpdateStamina", PrivateInstance);
        MethodInfo isSprinting = type.GetMethod("IsSprinting");

        stamina.SetValue(entity, 100f);
        sprinting.SetValue(entity, true);
        updateStamina.Invoke(entity, new object[] { 1f });
        Assert.That(stamina.GetValue(entity), Is.EqualTo(100f));
        Assert.That(isSprinting.Invoke(entity, null), Is.False);

        moving.SetValue(entity, true);
        updateStamina.Invoke(entity, new object[] { 1f });
        Assert.That((float)stamina.GetValue(entity), Is.LessThan(100f));

        stamina.SetValue(entity, 4f);
        updateStamina.Invoke(entity, new object[] { 1f });
        for (int i = 0; i < 12; i++)
        {
            updateStamina.Invoke(entity, new object[] { 1f });
        }
        Assert.That((float)stamina.GetValue(entity), Is.GreaterThan(3f));
        Assert.That(isSprinting.Invoke(entity, null), Is.False);

        sprinting.SetValue(entity, false);
        updateStamina.Invoke(entity, new object[] { 1f });
        sprinting.SetValue(entity, true);
        Assert.That(isSprinting.Invoke(entity, null), Is.True);
    }

    [Test]
    public void OpenGateDoesNotConsumeAnotherMatchingKey()
    {
        Type gateType = gameAssembly.GetType("Gate", true);
        Type entityType = gameAssembly.GetType("GameEntityObject", true);
        Type itemType = gameAssembly.GetType("Item+ItemData", true);
        Component gate = gameObject.AddComponent(gateType);
        Component player = gameObject.AddComponent(entityType);
        object item = Activator.CreateInstance(itemType);
        FieldInfo itemKind = itemType.GetField("Type");
        itemKind.SetValue(item, Enum.Parse(itemKind.FieldType, "KEY"));
        itemType.GetField("ParameterValues").SetValue(item, new List<float> { 7f });
        gateType.GetField("KeyID").SetValue(gate, 7);
        gateType.GetField("_isOpened", PrivateInstance).SetValue(gate, true);

        bool used = (bool)gateType.GetMethod("OnPlayerItemUse", PrivateInstance)
            .Invoke(gate, new[] { item, player });

        Assert.That(used, Is.False);
    }

    [Test]
    public void StaminaPotionAllowsSprintAfterExhaustion()
    {
        Type type = gameAssembly.GetType("GameEntityObject", true);
        Type itemType = gameAssembly.GetType("Item+ItemData", true);
        Component entity = gameObject.AddComponent(type);
        object item = Activator.CreateInstance(itemType);
        FieldInfo itemKind = itemType.GetField("Type");
        itemKind.SetValue(item, Enum.Parse(itemKind.FieldType, "STAMINA_POTION"));
        itemType.GetField("ParameterValues").SetValue(item, new List<float> { 30f });
        ((IList)type.GetField("_activePotions", PrivateInstance).GetValue(entity)).Add(item);
        type.GetField("_stamina", PrivateInstance).SetValue(entity, 0f);
        type.GetField("_isMoving", PrivateInstance).SetValue(entity, true);
        type.GetField("_isSprinting", PrivateInstance).SetValue(entity, true);
        type.GetField("_sprintExhausted", PrivateInstance).SetValue(entity, true);

        type.GetMethod("Update", PrivateInstance).Invoke(entity, null);

        Assert.That(type.GetMethod("IsSprinting").Invoke(entity, null), Is.True);
    }

    [TestCase("PlayerController")]
    [TestCase("Ghost")]
    [TestCase("Spider")]
    [TestCase("Skeleton")]
    [TestCase("Dragon")]
    public void FallingSpeedIsLimitedForPlayerAndEnemies(string typeName)
    {
        Type type = gameAssembly.GetType(typeName, true);
        Component controller = gameObject.AddComponent(type);
        type.GetField("Grounded").SetValue(controller, false);
        FieldInfo velocity = type.GetField("_verticalVelocity", PrivateInstance);
        velocity.SetValue(controller, -100f);

        type.GetMethod("UpdateGravity", PrivateInstance).Invoke(controller, null);

        Assert.That((float)velocity.GetValue(controller), Is.EqualTo(-53f));
    }
}
