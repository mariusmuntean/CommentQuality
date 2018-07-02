# CommentQuality

Estimate the quality of the comments on a YouTube video.

The backend uses Azure Functions to retrieve the comments from the YouTube Data API V3 and Azure Text Analytics API (Cognitive Services) to get a sentiment analysis.
The simple Fronend uses Ooui (Xamarin.Forms on Wasm) to consume the backend Rest API and paint a pretty UI.
