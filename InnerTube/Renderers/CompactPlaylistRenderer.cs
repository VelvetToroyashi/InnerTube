﻿using System.Text;
using Newtonsoft.Json.Linq;

namespace InnerTube.Renderers;

public class CompactPlaylistRenderer : IRenderer
{
	public string Type { get; }

	public string Id { get; }
	public string Title { get; }
	public IEnumerable<Thumbnail> Thumbnails { get; }
	public Channel Channel { get; }
	public int VideoCount { get; }
	public string FirstVideoId { get; }

	public CompactPlaylistRenderer(JToken renderer)
	{
		Type = renderer.Path.Split(".").Last();

		Id = renderer["playlistId"]!.ToString();
		Title = renderer["title"]!["simpleText"]!.ToString();
		VideoCount = int.Parse(renderer["videoCount"]!.ToString());

		Thumbnails =
			Utils.GetThumbnails(
				renderer.GetFromJsonPath<JArray>(
					"thumbnailRenderer.playlistVideoThumbnailRenderer.thumbnail.thumbnails")!);

		Channel = new Channel
		{
			Id = renderer.GetFromJsonPath<string>("longBylineText.runs[0].navigationEndpoint.browseEndpoint.browseId")!,
			Title = renderer.GetFromJsonPath<string>("longBylineText.runs[0].text")!,
			Avatar = null,
			Subscribers = null,
			Badges = renderer.GetFromJsonPath<JArray>("ownerBadges")
				?.Select(x => new Badge(x["metadataBadgeRenderer"]!)) ?? Array.Empty<Badge>()
		};

		FirstVideoId = renderer.GetFromJsonPath<string>("navigationEndpoint.watchEndpoint.videoId")!;
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder()
			.AppendLine($"[{Type}] {Title}")
			.AppendLine($"- Id: {Id}")
			.AppendLine($"- VideoCount: {VideoCount}")
			.AppendLine($"- Thumbnail count: {Thumbnails.Count()}")
			.AppendLine($"- Channel: {Channel}")
			.AppendLine($"- FirstVideoId: {FirstVideoId}");

		return sb.ToString();
	}
}