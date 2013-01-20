package tv.ouya.sdk;
import tv.ouya.console.api.*;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.ListView;
import android.widget.Toast;
import tv.ouya.console.api.CancelIgnoringOuyaResponseListener;
import tv.ouya.console.api.OuyaController;
import tv.ouya.console.api.OuyaEncryptionHelper;
import tv.ouya.console.api.OuyaFacade;
import tv.ouya.console.api.OuyaResponseListener;
import tv.ouya.console.api.Product;
import tv.ouya.console.api.Purchasable;
import tv.ouya.console.api.Receipt;
import tv.ouya.console.internal.util.Strings;

import com.google.gson.Gson;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

import com.unity3d.player.UnityPlayer;

public class TestOuyaFacade
{
    /*
     * Log onto the developer website (you should have received a URL, a username and a password in email)
     * and get your developer ID. Plug it in here. Use your developer ID, not your developer UUID.
     *
     * The current value of 3 is our developer account. You should change this.
     */
    public static final String DEVELOPER_ID = "310a8f51-4d6e-4ae5-bda0-b93878e5f5d0";

    /*
     * Before this app will run, you must define some purchasable items on the developer website. Once
     * you have defined those items, put their SKUs in the List below.
     *
     * The SKUs below are those in our developer account. You should change them.
     */
    public static final List<Purchasable> PRODUCT_IDENTIFIER_LIST = Arrays.asList(new Purchasable("long_sword"), new Purchasable("sharp_axe"), new Purchasable("__DECLINED__THIS_PURCHASE"));

    /*
     * The SKU adapter will display a purchasable item in a cell in a ListView. It's not part of the in-app
     * purchase API. Neither is the ListView itself.
     */
    //private SkuAdapter skuAdapter;
    //private ListView skuListView;

    /*
     * The receipt adapter will display a previously-purchased item in a cell in a ListView. It's not part of the in-app
     * purchase API. Neither is the ListView itself.
     */
    //private ReceiptAdapter receiptAdapter;
    //private ListView receiptListView;

	//android context
	private Context context;

	/*
     * Your game talks to the OuyaFacade, which hides all the mechanics of doing an in-app purchase.
     */
    private OuyaFacade ouyaFacade = new OuyaFacade();

	public TestOuyaFacade(Context context)
	{
		this.context = context;

		Log.i("TestOuyaFacade", "TestOuyaFacade.Init();");
		UnityPlayer.UnitySendMessage("OuyaGameObject", "DebugLog", "TestOuyaFacade.Init();");
		Init();
	}

	private void setupSkuList() {
        //skuAdapter = new SkuAdapter(this, new RequestPurchaseClickListener());
        //skuListView = (ListView) findViewById(R.id.skus);
        //skuListView.setAdapter(skuAdapter);
    }

	private void Init()
	{
		Log.i("TestOuyaFacade", "OuyaFacade.init(context, DEVELOPER_ID);");
		UnityPlayer.UnitySendMessage("OuyaGameObject", "DebugLog", "ouyaFacade.init(context, DEVELOPER_ID);");
        ouyaFacade.init(context, DEVELOPER_ID);

        //setupSkuList();

        //receiptAdapter = new ReceiptAdapter(this);
        //receiptListView = (ListView) findViewById(R.id.receipts);
        //receiptListView.setAdapter(receiptAdapter);

        /*
         * In order to avoid "application not responding" popups, Android demands that long-running operations
         * happen on a background thread. Listener objects provide a way for you to specify what ought to happen
         * at the end of the long-running operation. Examples of this pattern in Android include
         * android.os.AsyncTask.
         */
        Log.i("TestOuyaFacade", "OuyaResponseListener<ArrayList<Product>> productListListener = new CancelIgnoringOuyaResponseListener<ArrayList<Product>>() {");
        UnityPlayer.UnitySendMessage("OuyaGameObject", "DebugLog", "OuyaResponseListener<ArrayList<Product>> productListListener = new CancelIgnoringOuyaResponseListener<ArrayList<Product>>() {");
        m_productListListener = new CancelIgnoringOuyaResponseListener<ArrayList<Product>>() {
            @Override
            public void onSuccess(ArrayList<Product> products) {
                addProducts(products);
            }

            @Override
            public void onFailure(int errorCode, String errorMessage)
			{
                // Your app probably wants to do something more sophisticated than popping a Toast. This is
                // here to tell you that your app needs to handle this case: if your app doesn't display
                // something, the user won't know of the failure.
                //Toast.makeText(OuyaSampleActivity.this, "Could not fetch product information (error " + errorCode + ": " + errorMessage + ")", Toast.LENGTH_LONG).show();
				Log.i("TestOuyaFacade", "Could not fetch product information (error " + errorCode + ": " + errorMessage + ")");
				UnityPlayer.UnitySendMessage("OuyaGameObject", "DebugLogError", "Could not fetch product information (error " + errorCode + ": " + errorMessage + ")");
            }
        };

		/*
        findViewById(R.id.gamer_uuid_button).setOnClickListener(new View.OnClickListener()
		{
            @Override
            public void onClick(View v) {
                ouyaFacade.requestGamerUuid(new CancelIgnoringOuyaResponseListener<String>() {
                    @Override
                    public void onSuccess(String result)
					{
                        // new AlertDialog.Builder(OuyaSampleActivity.this)
                        //    .setTitle(getString(R.string.alert_title))
                        //    .setMessage(result)
                        //    .show();
                    }

                    @Override
                    public void onFailure(int errorCode, String errorMessage)
					{
                        //Toast.makeText(OuyaSampleActivity.this, "Unable to fetch gamer UUID (error " + errorCode + ": " + errorMessage + ")", Toast.LENGTH_LONG).show();
                    }
                });
            }
        });
		*/

        //fetchReceipts();
    }
	
	private OuyaResponseListener<ArrayList<Product>> m_productListListener = null;
    
    public void getProductsAsync()
    {
    	Log.i("TestOuyaFacade", "ouyaFacade.requestProductList(SKU_LIST, productListListener);");
		UnityPlayer.UnitySendMessage("OuyaGameObject", "DebugLog", "ouyaFacade.requestProductList(SKU_LIST, productListListener);");
        ouyaFacade.requestProductList(PRODUCT_IDENTIFIER_LIST, m_productListListener);
    }

    private void fetchReceipts()
	{
        ouyaFacade.requestReceipts(new CancelIgnoringOuyaResponseListener<String>()
		{
            @Override
            public void onSuccess(String receiptResponse)
			{
                OuyaEncryptionHelper helper = new OuyaEncryptionHelper();
                List<Receipt> receipts = null;
                try {
                    receipts = helper.decryptReceiptResponse(receiptResponse);
                } catch (IOException e) {
                    throw new RuntimeException(e);
                }
                Collections.sort(receipts, new Comparator<Receipt>() {
                    @Override
                    public int compare(Receipt lhs, Receipt rhs) {
                        return rhs.getPurchaseDate().compareTo(lhs.getPurchaseDate());
                    }
                });
                //receiptAdapter.set(receipts);
            }

            @Override
            public void onFailure(int errorCode, String errorMessage) {
                // Again, your game should do something more sophisticated.
                //Toast.makeText(OuyaSampleActivity.this, "Could not fetch receipts (error " + errorCode + ": " + errorMessage + ")", Toast.LENGTH_LONG).show();
            }
        });
    }

	public void addProducts(List<Product> products)
	{
        //skuAdapter.add(products);
		for (Product p : products)
		{
			Gson gson = new Gson();
			String jsonData = gson.toJson(p);

			Log.i("TestOuyaFacade", "AddProduct jsonData=" + jsonData);
			UnityPlayer.UnitySendMessage("OuyaGameObject", "ProductListListener", jsonData);
		}
    }

	/*
     * This will be called when the user clicks on an item in the ListView.
     */
    public void requestPurchase(final String productId)
	{
        ouyaFacade.requestPurchase(new Purchasable(productId), new CancelIgnoringOuyaResponseListener<Product>()
		{
            @Override
            public void onSuccess(Product product)
			{
				/*
                new AlertDialog.Builder(OuyaSampleActivity.this)
                        .setTitle(getString(R.string.alert_title))
                        .setMessage("You have successfully purchased a " + product.getName() + " for " + Strings.formatDollarAmount(product.getPriceInCents()))
                        .show();
				*/
                //fetchReceipts();
            	
            	Log.i("OuyaGameObject", "requestPurchase onSuccess You have successfully purchased a " + product.getName() + " for " + Strings.formatDollarAmount(product.getPriceInCents()));
        		UnityPlayer.UnitySendMessage("OuyaGameObject", "DebugLog", "requestPurchase onSuccess You have successfully purchased a " + product.getName() + " for " + Strings.formatDollarAmount(product.getPriceInCents()));
            }

            @Override
            public void onFailure(int errorCode, String errorMessage)
			{
				/*
                new AlertDialog.Builder(OuyaSampleActivity.this)
                        .setTitle(getString(R.string.alert_title))
                        .setMessage("Unfortunately, your purchase failed [error code " + errorCode + " (" + errorMessage + ")]. Would you like to try again?")
                        .setPositiveButton(R.string.ok, new DialogInterface.OnClickListener()
						{
                            @Override
                            public void onClick(DialogInterface dialogInterface, int i)
							{
                                dialogInterface.dismiss();
                                requestPurchase(sku);
                            }
                        })
                        .setNegativeButton(R.string.cancel, null)
                        .show();
				*/
            	
            	Log.i("OuyaGameObject", "requestPurchase onFailure errorMessage=" + errorMessage);
        		UnityPlayer.UnitySendMessage("OuyaGameObject", "DebugLogError", "requestPurchase onFailure errorMessage=" + errorMessage);
            }
        });
    }
}