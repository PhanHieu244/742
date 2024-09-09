using System.Collections;
using System.Collections.Generic;
using SgLib;
using UnityEngine;

public class PurchasingManager : MonoBehaviour
{
   public void OnPressDown(int i)
   {
      switch (i)
      {
         case 1:
            AddDiamond(1);
             IAPManager.Instance.BuyProductID(IAPKey.PACK1);
            break;
         case 2:
            AddDiamond(3);
            IAPManager.Instance.BuyProductID(IAPKey.PACK2);
            break;
         case 3:
            AddDiamond(5);
            IAPManager.Instance.BuyProductID(IAPKey.PACK3);
            break;
         case 4:
            AddDiamond(10);
            IAPManager.Instance.BuyProductID(IAPKey.PACK4);
            break;
      }
   }

   public void AddDiamond(int a){
      CoinManager.Instance.AddCoins(a);
   }

   public void Sub(int i)
   {
      GameDataManager.Instance.playerData.SubDiamond(i);
   }
}
