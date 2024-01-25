using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// ScriptableObject 会在 Unity 编辑器中创建一个 Asset 文件，可以在多个 GameObject 之间共享数据
// CharacterEventSO 代表要传递 Character 类型的参数
// [CreateAssetMenu(menuName = "Event/CharacterEventSO", fileName = "CharacterEventSO")]
// public class CharacterEventSO : ScriptableObject
// {
//     // Unity 的事件委托，可以在 Unity 编辑器中添加多个事件监听器
//     // 该事件可以被任何一个 GameObject 监听
//     // 只要订阅了该事件的 GameObject，当事件触发时，都会调用 RaiseEvent
//     public UnityAction<Character> OnEventRaised;

//     // 事件触发时调用该方法
//     public void RaiseEvent(Character character)
//     {
//         OnEventRaised?.Invoke(character);
//     }
// }
