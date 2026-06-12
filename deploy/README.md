# Deploy (Docker / Kubernetes)

Backend services for Iron Exiles are implemented as **containers** per `.adlc/ETHOS.md` §7:

- **Local development:** Docker Compose (see REQ-042 Auth stack)
- **Production:** Kubernetes manifests under `deploy/k8s/` (added in REQ-042+)

The UE5 game client is not containerized for player installs. Dedicated game servers and backend microservices are.
