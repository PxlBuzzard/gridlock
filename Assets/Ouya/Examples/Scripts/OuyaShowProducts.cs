using System;
using System.Collections.Generic;
using UnityEngine;

public class OuyaShowProducts : MonoBehaviour
{
    /// <summary>
    /// This is your assigned developer id
    /// </summary>
    private const string DEVELOPER_ID = "310a8f51-4d6e-4ae5-bda0-b93878e5f5d0";

    void Awake()
    {
        try
        {
            OuyaSDK.initialize(DEVELOPER_ID);

            OuyaSDK.registerProductListListener(new OuyaSDK.CancelIgnoringIapResponseListener<List<OuyaSDK.Product>>()
            {
                onSuccess = (List<OuyaSDK.Product> products) =>
                {
                    AddProductsHandler(products);
                },

                onFailure = (int errorCode, string errorMessage) =>
                {
                    // Your app probably wants to do something more sophisticated than popping a Toast. This is
                    // here to tell you that your app needs to handle this case: if your app doesn't display
                    // something, the user won't know of the failure.
                    Debug.Log(string.Format("Could not fetch product information (error {0}: {1})", errorCode, errorMessage));
                }
            });

            OuyaSDK.registerRequestPurchaseListener(new OuyaSDK.CancelIgnoringIapResponseListener<OuyaSDK.Product>()
            {
                onSuccess = (OuyaSDK.Product product) =>
                {
                    Debug.Log(string.Format("requestPurchase onSuccess You have successfully purchased a {0} for {1}", product.getName(), product.getPriceInCents()));
                },

                onFailure = (int errorCode, string errorMessage) =>
                {
                    // Your app probably wants to do something more sophisticated than popping a Toast. This is
                    // here to tell you that your app needs to handle this case: if your app doesn't display
                    // something, the user won't know of the failure.
                    Debug.Log(string.Format("Could not fetch product information (error {0}: {1})", errorCode, errorMessage));
                }
            });

            // request initial product list
            OuyaSDK.requestProductList(OuyaSDK.PRODUCT_IDENTIFIER_LIST, OuyaSDK.getProductListListener());
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("Awake exception={0}", ex));
        }
    }

    public void AddProductsHandler(List<OuyaSDK.Product> products)
    {
        try
        {
            foreach (OuyaSDK.Product product in products)
            {
                Debug.Log("Product identifier=" + product.getIdentifier());
                Debug.Log("Product name=" + product.getName());
                Debug.Log("Product price=" + product.getPriceInCents());
                if (!m_products.ContainsKey(product.getIdentifier()))
                {
                    m_products[product.getIdentifier()] = product;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("Failed to addProducts exception={0}", ex));
        }
    }

    #region Data containers

    private Dictionary<string, OuyaSDK.Product> m_products = new Dictionary<string, OuyaSDK.Product>();

    #endregion

    #region Presentation

    private void OnGUI()
    {
        try
        {
            GUILayout.Label("Products:");

            if (GUILayout.Button("Get Products", GUILayout.Height(40)))
            {
                OuyaSDK.requestProductList(OuyaSDK.PRODUCT_IDENTIFIER_LIST, OuyaSDK.getProductListListener());
            }

            foreach (KeyValuePair<string, OuyaSDK.Product> kvp in m_products)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(string.Format("Name={0}", kvp.Value.getName()));
                GUILayout.Label(string.Format("Price={0}", kvp.Value.getPriceInCents()));
                GUILayout.Label(string.Format("Identifier={0}", kvp.Value.getIdentifier()));

                if (GUILayout.Button("Purchase"))
                {
                    Debug.Log(string.Format("Purchase Identifier: {0}", kvp.Value.getIdentifier()));
                    OuyaSDK.requestPurchase(kvp.Value.getIdentifier(), OuyaSDK.getRequestPurchaseListener());
                }

                GUILayout.EndHorizontal();
            }
        }
        catch (System.Exception)
        {
        }
    }

    #endregion
}