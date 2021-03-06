using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-03-06 PM 4:33:43
// 작성자 : Rito

/*
    [목록]

    - CamToMouse(hit, distance, layer) : 현재 활성화된 카메라로부터 마우스 위치에 레이캐스트
    - SkyToPoint(hit, point, height, layer) : Y축 상단으로 height만큼 떨어진 지점에서 대상 지점으로 레이캐스트
    - IsCursorOverUI() : 마우스 커서가 UI 위에 있는지 여부 검사
*/

// TODO : Debug.DrawRay 편하게 쓸 수 있도록

namespace Rito.UnityLibrary
{
    public static class Raycaster
    {
        public const float MaxRayDistance = 9999f;

        /***********************************************************************
        *                               Raycast 3D
        ***********************************************************************/
        #region .
        /// <summary> 현재 활성화된 카메라로부터 마우스 위치에 레이캐스트 </summary>
        public static bool CamToMouse(out RaycastHit hit, in float distance = MaxRayDistance, in int layerMask = -1,
            QueryTriggerInteraction interaction = QueryTriggerInteraction.Collide)
        {
            return Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, distance, layerMask, interaction);
        }

        /// <summary> Y축 상단으로 height만큼 떨어진 지점에서 대상 지점으로 레이캐스트 </summary>
        public static bool SkyToPoint(out RaycastHit hit, in Vector3 point, in float height = MaxRayDistance,
            in int layerMask = -1, QueryTriggerInteraction interaction = QueryTriggerInteraction.Collide)
        {
            Ray ray = new Ray(new Vector3(point.x, point.y + height, point.x), Vector3.down);
            return Physics.Raycast(ray, out hit, height, layerMask, interaction);
        }

        #endregion

        /***********************************************************************
        *                               UI Raycast
        ***********************************************************************/
        #region .

        /// <summary>
        /// 마우스 커서가 UI 위에 있는지 검사 <para/>
        /// 이벤트 시스템이 없는 경우, 빈 게임오브젝트를 생성해서 자동으로 추가
        /// </summary>
        public static bool IsCursorOverUI()
        {
            // 이벤트 시스템이 없는 경우 생성
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                new GameObject("EventSystem",
                    typeof(UnityEngine.EventSystems.EventSystem),
                    typeof(UnityEngine.EventSystems.StandaloneInputModule)
                );
            }

            return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        }

        #endregion
    }
}