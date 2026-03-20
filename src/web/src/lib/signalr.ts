import * as signalR from "@microsoft/signalr";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL;

export function createHubConnection(hubPath: string) {
  return new signalR.HubConnectionBuilder()
    .withUrl(`${API_BASE_URL}${hubPath}`)
    .withAutomaticReconnect()
    .build();
}
