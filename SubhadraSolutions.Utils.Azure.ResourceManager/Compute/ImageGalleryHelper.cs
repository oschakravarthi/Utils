using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.Azure.ResourceManager.Compute;

public static class ImageGalleryHelper
{
    public static async IAsyncEnumerable<VirtualMachineImageBaseAndSku> GetImageListAsync(this ArmClient armClient,
        string region, string publisherName, string offerName, string skuName = null)
    {
        var subscription = await armClient.GetDefaultSubscriptionAsync();
        var images = GetImageListAsync(subscription, region, publisherName, offerName, skuName);
        await foreach (var image in images)
        {
            yield return image;
        }
    }

    public static async IAsyncEnumerable<VirtualMachineImageBaseAndSku> GetImageListAsync(
        this SubscriptionResource subscription, string region, string publisherName, string offerName,
        string skuName = null)
    {
        var location = new AzureLocation(region);
        if (location.DisplayName == null)
        {
            throw new ArgumentException($"Azure region {region} not found");
        }

        var publishers = subscription.GetVirtualMachineImagePublishers(location);
        var publisher = publishers
            .FirstOrDefault(x => string.Equals(x.Name, publisherName, StringComparison.CurrentCultureIgnoreCase));
        if (publisher == null)
        {
            throw new ArgumentException($"Publisher {publisherName} not found in Azure region {region}");
        }

        var offers = subscription.GetVirtualMachineImageOffersAsync(location, publisher.Name);

        VirtualMachineImageBase offer = null;
        await foreach (var page in offers.AsPages())
        {
            var shouldBreak = false;
            // enumerate through page items
            foreach (var image in page.Values)
            {
                if (string.Equals(image.Name, offerName, StringComparison.CurrentCultureIgnoreCase))
                {
                    offer = image;
                    shouldBreak = true;
                    break;
                }
            }

            if (shouldBreak)
            {
                break;
            }
        }

        //.Where(x => string.Equals(x.Name, offerName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
        if (offer == null)
        {
            throw new ArgumentException(
                $"Offer {offerName} not available for Publisher {publisherName} in Azure region {region}");
        }

        if (string.IsNullOrEmpty(skuName))
        {
            var skus = subscription.GetVirtualMachineImageSkusAsync(location, publisher.Name, offer.Name);

            await foreach (var page in skus.AsPages())
            {
                foreach (var sku in page.Values)
                {
                    var images =
                        subscription.GetVirtualMachineImagesAsync(location, publisher.Name, offer.Name, sku.Name);
                    var set = GetImages(images);
                    await foreach (var image in set)
                    {
                        yield return new VirtualMachineImageBaseAndSku(image, sku.Name);
                    }
                }
            }
        }
        else
        {
            var skus = subscription.GetVirtualMachineImageSkusAsync(location, publisher.Name, offer.Name);
            VirtualMachineImageBase sku = null;
            await foreach (var page in skus.AsPages())
            {
                var shouldBreak = false;

                foreach (var sk in page.Values)
                {
                    if (string.Equals(sk.Name, skuName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        sku = sk;
                        shouldBreak = true;
                        break;
                    }
                }

                if (shouldBreak)
                {
                    break;
                }
            }

            if (sku == null)
            {
                throw new ArgumentException(
                    $"Sku {skuName} not available for Offer {offerName} and Publisher {publisherName} in Azure region {region}");
            }

            var images = subscription.GetVirtualMachineImagesAsync(location, publisher.Name, offer.Name, sku.Name);

            var set = GetImages(images);
            await foreach (var image in set)
            {
                yield return new VirtualMachineImageBaseAndSku(image, sku.Name);
            }
        }
    }

    private static async IAsyncEnumerable<VirtualMachineImageBase> GetImages(
        AsyncPageable<VirtualMachineImageBase> images)
    {
        await foreach (var page1 in images.AsPages())
        {
            foreach (var image in page1.Values)
            {
                yield return image;
            }
        }
    }
}